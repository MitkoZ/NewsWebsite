﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;
using Services.CRUD.Interfaces;
using NewsWebsite.ViewModels.News.Comments;
using NewsWebsite.Utils;
using Services.Transactions.Interfaces;
using NewsWebsite.ViewModels.Users;

namespace NewsWebsite.Controllers.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICommentsService commentsService;
        private readonly IUsersService userService;

        public CommentsController(IUnitOfWork unitOfWork, ICommentsService commentsService, IUsersService usersService)
        {
            this.unitOfWork = unitOfWork;
            this.commentsService = commentsService;
            this.userService = usersService;
        }

        private async Task<GetCommentViewModel> MapToGetCommentViewModelAsync(Comment commentDb)
        {
            return new GetCommentViewModel
            {
                Id = commentDb.Id,
                Content = await this.commentsService.ReplaceUserIdsWithUsernames(commentDb.Content),
                ParentId = commentDb.ParentId,
                Fullname = commentDb.User.UserName,
                CreatedAt = commentDb.CreatedAt,
                UpdatedAt = commentDb.UpdatedAt,
                Creator = commentDb.UserId,
                CreatedByCurrentUser = this.IsCreatedByCurrentUser(commentDb.UserId),
                Pings = this.commentsService.GetPingedUsers(commentDb.Content),
                UpvoteCount = commentsService.GetVotes(commentDb).Count,
                UserHasUpvoted = commentsService.GetVotes(commentDb, vote => vote.UserId == this.GetCurrentUserId()).Any()
            };
        }

        [HttpGet("{newsId}")]
        public async Task<ActionResult<List<GetCommentViewModel>>> GetCommentsAsync([FromRoute]string newsId)
        {
            List<Comment> commentsDb = await commentsService.GetAll(x => x.NewsId == newsId)
                                                            .ToListAsync();

            if (commentsDb.Count == 0)
            {
                return Ok(new List<GetCommentViewModel>()); // jquery-comments expects an empty collection
            }

            List<GetCommentViewModel> commentViewModels = new List<GetCommentViewModel>();

            foreach (Comment commentDb in commentsDb)
            {
                commentViewModels.Add(await this.MapToGetCommentViewModelAsync(commentDb)); //TODO: do we really need last modified date and created date??? TODO: use MapToGetCommentViewModel
            }

            return Ok(commentViewModels);
        }

        //private string GetPings(string content)
        //{
        //    return content.Split('@').ToList().ForEach(userId => this.userService.GetAll().FirstOrDefault(user => user.Id == userId));
        //}

        private bool IsCreatedByCurrentUser(string userId)
        {
            return this.GetCurrentUserId() == userId ? true : false;
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<EditCommentViewModel>> GetComment(string id)
        //{
        //    Comment commentDb = await commentsService.GetAll(x => x.Id == id).FirstOrDefaultAsync();

        //    if (commentDb == null)
        //    {
        //        return NotFound();
        //    }

        //    EditCommentViewModel commentViewModel = new EditCommentViewModel //TODO: do we really need last modified date and created date???
        //    {
        //        Id = commentDb.Id,
        //        Content = commentDb.Content,
        //        Fullname = commentDb.User.UserName,
        //        ParentId = commentDb.ParentId
        //    };

        //    return commentViewModel;
        //}

        [HttpPut("{id}")]
        public async Task<ActionResult<GetCommentViewModel>> PutComment(PutCommentViewModel putCommentViewModel)
        {

            Comment commentDb = await commentsService.GetAll(x => x.Id == putCommentViewModel.Id)
                                                     .FirstOrDefaultAsync();

            commentDb.Content = putCommentViewModel.Content;
            this.commentsService.Save(commentDb);
            bool isSaved = await unitOfWork.CommitAsync();

            if (isSaved)
            {
                GetCommentViewModel getCommentViewModel = await this.MapToGetCommentViewModelAsync(commentDb);
                return getCommentViewModel;
            }

            return BadRequest();
        }

        [HttpPost("{newsId}")]
        public async Task<ActionResult<GetCommentViewModel>> PostComment([FromRoute]string newsId, PostCommentViewModel postCommentViewModel)
        {
            Comment commentDb = new Comment
            {
                NewsId = newsId,
                Content = postCommentViewModel.Content,
                ParentId = postCommentViewModel.ParentId
            };

            User currentUserDb = await this.userService.GetAll(x => x.Id == this.GetCurrentUserId())
                                                        .FirstOrDefaultAsync();
            currentUserDb.Comments.Add(commentDb);

            commentsService.Save(commentDb);
            bool isSaved = await this.unitOfWork.CommitAsync();

            if (isSaved)
            {
                GetCommentViewModel commentViewModel = await this.MapToGetCommentViewModelAsync(commentDb);

                return CreatedAtAction(nameof(GetCommentsAsync), new { id = commentDb.Id }, commentViewModel);// TODO: should return getCommentViewModel, remove created at action
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Comment>> DeleteComment([FromRoute]string id)
        {
            await this.commentsService.DeleteAsync(id);
            bool isDeleted = await this.unitOfWork.CommitAsync();

            if (isDeleted)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpPost("{id}/upvote")]
        public async Task<ActionResult<GetCommentViewModel>> UpvoteAsync([FromRoute] string id)
        {
            Comment commentDb = await this.commentsService.GetAll(x => x.Id == id)
                                                          .FirstOrDefaultAsync();
            if (commentDb == null)
            {
                return NotFound();
            }

            if (this.commentsService.IsUserVotedForComment(this.GetCurrentUserId(), commentDb)) // this user has already votes for the current comment
            {
                return BadRequest();
            }

            commentsService.SaveVote(commentDb, new Vote { UserId = this.GetCurrentUserId(), CommentId = id });
            bool isSaved = await unitOfWork.CommitAsync();
            if (isSaved)
            {
                GetCommentViewModel getCommentViewModel = await this.MapToGetCommentViewModelAsync(commentDb);
                return Ok(getCommentViewModel);
            }

            return BadRequest();
        }

        [HttpDelete("{id}/downvote")]
        public async Task<ActionResult<GetCommentViewModel>> DownvoteAsync([FromRoute] string id)
        {
            Comment commentDb = await this.commentsService.GetAll(x => x.Id == id)
                                                          .FirstOrDefaultAsync();
            if (commentDb == null)
            {
                return NotFound();
            }

            if (this.commentsService.IsUserVotedForComment(this.GetCurrentUserId(), commentDb))
            {
                Vote currentVote = commentDb.Votes.FirstOrDefault(vote => vote.UserId == this.GetCurrentUserId());
                this.commentsService.DeleteVote(currentVote);
            }

            bool isSaved = await unitOfWork.CommitAsync();
            if (isSaved)
            {
                GetCommentViewModel getCommentViewModel = await this.MapToGetCommentViewModelAsync(commentDb);
                return Ok(getCommentViewModel);
            }

            return BadRequest();
        }

        [HttpGet("/api/users/{username}")] // Please note that the "/" part of the action attribute overrides the controller's attribute, so it does not append it's template
        public async Task<ActionResult<List<GetUserViewModel>>> GetUsersByUsernameFilter([FromRoute]string username)//TODO: should we include the newsId, so we can show ping for users that have only already commented the current news?
        {
            List<GetUserViewModel> usersViewModels = new List<GetUserViewModel>();

            List<User> usersDb = await this.userService.GetAll(userDb => userDb.UserName.ToLower().Contains(username.ToLower()))
                                                       .ToListAsync();

            usersDb.ForEach(userDb => usersViewModels.Add(
                new GetUserViewModel
                {
                    Id = userDb.Id,
                    Fullname = userDb.UserName,
                    Email = userDb.Email
                }
             )
            );

            return usersViewModels;
        }
    }
}
