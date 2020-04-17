using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Auth.Interfaces;
using Services.CRUD.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.CRUD
{
    public class CommentsService : BaseCRUDService<Comment>, ICommentsService
    {
        private readonly IUsersService usersService;

        public CommentsService(ICommentsRepository repository, IUsersService usersService) : base(repository)
        {
            this.usersService = usersService;
        }

        public override void Delete(string id)
        {
            List<Comment> subCommentsDb = base.repository.GetAll(x => x.ParentId == id)
                                                         .ToList(); // if the collection is empty, the foreach won't execute

            subCommentsDb.ForEach(subComment =>
            {
                this.Delete(subComment.Id); // deletes subcomments recursively
            });

            base.Delete(id); // deletes main comment
        }

        public Dictionary<string, string> GetPingedUsers(string content)
        {
            Dictionary<string, string> idsUsersDictionary = new Dictionary<string, string>();
            List<string> pingedUsersIds = this.GetPingedUsersIds(content);

            pingedUsersIds.ForEach(userId =>
            {
                User userDb = this.usersService.GetAll(userDb => userDb.Id == userId)
                                               .FirstOrDefault(); // don't use async FirstOrDefault, since it doesn't play well with ForEach (https://stackoverflow.com/questions/18667633/how-can-i-use-async-with-foreach, https://markheath.net/post/async-antipatterns "The ForEach method accepts an Action<T>, which returns void. So you've essentially created an async void method, which of course was one of our previous antipatterns as the caller has no way of awaiting it.")

                if (!idsUsersDictionary.ContainsKey(userDb.Id))
                {
                    idsUsersDictionary.Add(userDb.Id, userDb.UserName);
                }

            });

            return idsUsersDictionary;
        }

        private List<string> GetPingedUsersIds(string content)
        {
            List<string> pingedUsersIds = new List<string>();
            int idLength = Guid.NewGuid().ToString().Length;

            for (int i = 0; i < content.Length; i++)
            {
                char character = content[i];

                int charactersAmountAfterPingCharacter = content.Length - (i + 1);
                if (character == '@' && charactersAmountAfterPingCharacter >= idLength)
                {
                    string probableGuid = content.Substring(i + 1, idLength);
                    if (Guid.TryParse(probableGuid, out Guid sampleGuid))
                    {
                        string userId = probableGuid;
                        pingedUsersIds.Add(userId);
                    }
                }
            }

            return pingedUsersIds;
        }

        public async Task<string> ReplaceUserIdsWithUsernames(string content)
        {
            List<User> usersDb = await this.usersService.GetAll()
                                                        .ToListAsync();

            foreach (User userDb in usersDb)
            {
                content = content.Replace("@" + userDb.Id, "@" + userDb.UserName);
            }

            return content;
        }

        public bool IsUserVotedForComment(string userId, Comment commentDb)
        {
            return this.GetVotes(commentDb).Any(vote => vote.UserId == userId);
        }

        public void DeleteVote(Vote vote)
        {
            vote.IsDeleted = true;
        }

        public List<Vote> GetVotes(Comment commentDb, Func<Vote, bool> filter = null, bool isQueryDeletedRecords = false)
        {
            Func<Vote, bool> finalFilter;
            Func<Vote, bool> isQueryDeletedRecordsFilter = x => x.IsDeleted == isQueryDeletedRecords;

            if (filter == null)
            {
                finalFilter = isQueryDeletedRecordsFilter;
            }
            else
            {
                finalFilter = x => isQueryDeletedRecordsFilter(x) && filter(x);
            }

            return commentDb.Votes.Where(finalFilter).ToList();
        }

        public void SaveVote(Comment commentDb, Vote vote)
        {
            if (this.GetVotes(commentDb, isQueryDeletedRecords: true).Any(x => x.UserId == vote.UserId)) // it's already contained in the database and we should undelete it
            {
                this.UndeleteVote(commentDb.Votes.FirstOrDefault(x => x.UserId == vote.UserId));
            }
            else // we have to insert it
            {
                this.InsertVote(commentDb, vote);
            }
        }

        private void InsertVote(Comment commentDb, Vote vote)
        {
            commentDb.Votes.Add(vote);
        }

        private void UndeleteVote(Vote vote)
        {
            vote.IsDeleted = false;
        }

        public async Task<string> GetOwnerId(string itemId)
        {
            return (await this.repository
                .GetAll(comment => comment.Id == itemId)
                .FirstOrDefaultAsync())
                .UserId;
        }
    }
}
