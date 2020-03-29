using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;
using Services.CRUD.Interfaces;
using NewsWebsite.ViewModels;
using NewsWebsite.ViewModels.News.Comments;
using System.Security.Claims;
using NewsWebsite.Utils;
using System.Diagnostics;

namespace NewsWebsite.Controllers.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService commentsService;
        private readonly IUsersService userService;

        public CommentsController(ICommentsService commentsService, IUsersService usersService)
        {
            this.commentsService = commentsService;
            this.userService = usersService;
        }

        [HttpGet("{newsId}")]
        public async Task<ActionResult<List<EditCommentViewModel>>> GetComments(string newsId)
        {
            List<Comment> commentsDb = await commentsService.GetAll(x => x.NewsId == newsId)
                                                            .ToListAsync();

            if (commentsDb.Count == 0)
            {
                return Ok(new List<EditCommentViewModel>()); // jquery-comments expects an empty collection
            }

            List<EditCommentViewModel> commentViewModels = new List<EditCommentViewModel>();

            foreach (Comment commentDb in commentsDb)
            {
                commentViewModels.Add(new EditCommentViewModel //TODO: do we really need last modified date and created date???
                {
                    Id = commentDb.Id,
                    Content = commentDb.Content,
                    Fullname = commentDb.User.UserName,
                    ParentId = commentDb.ParentId,
                    CreatedAt = commentDb.CreatedAt,
                    UpdatedAt = commentDb.UpdatedAt
                });
            }

            return Ok(commentViewModels);
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
        public async Task<IActionResult> PutComment(string id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            Comment commentDb = await commentsService.GetAll(x => x.Id == id)
                                                     .FirstOrDefaultAsync();

            bool isSaved = await commentsService.SaveAsync(commentDb);
            if (isSaved)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpPost("{newsId}")]
        public async Task<ActionResult<Comment>> PostComment([FromRoute]string newsId, CreateCommentViewModel createCommentViewModel)
        {
            Comment commentDb = new Comment
            {
                NewsId = newsId,
                Content = createCommentViewModel.Content,
                ParentId = createCommentViewModel.ParentId
            };

            User currentUserDb = await this.userService.GetAll(x => x.Id == this.GetCurrentUserId()).FirstOrDefaultAsync();
            currentUserDb.Comments.Add(commentDb);

            bool isSaved = await commentsService.SaveAsync(commentDb);

            if (isSaved)
            {
                EditCommentViewModel commentViewModel = new EditCommentViewModel //TODO: do we really need last modified date and created date???
                {
                    Id = commentDb.Id,
                    Content = commentDb.Content,
                    ParentId = commentDb.ParentId,
                    Fullname = commentDb.User.UserName
                };

                return CreatedAtAction(nameof(GetComments), new { id = commentDb.Id }, commentViewModel);// TODO: should return getCommentViewModel, remove created at action
            }

            return BadRequest();
        }

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Comment>> DeleteComment(string id)
        //{
        //    var comment = await _context.Comments.FindAsync(id);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Comments.Remove(comment);
        //    await _context.SaveChangesAsync();

        //    return comment;
        //}

    }
}
