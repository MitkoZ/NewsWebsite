using DataAccess.Entities;
using Services.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.CRUD.Interfaces
{
    public interface ICommentsService : IBaseCRUDService<Comment>, IItemLookup
    {
        /// <summary>
        /// Gets the pinged users of the content of a comment
        /// </summary>
        /// <param name="content">The actual content of the comment that may contain pinged users in the @userId format</param>
        /// <returns>userId as the key and username as the value</returns>
        Dictionary<string, string> GetPingedUsers(string content);

        /// <summary>
        /// Replaces the user ids with their corresponding usernames
        /// </summary>
        /// <param name="content">The actual content of the comment that may contain pinged users in the @userId format</param>
        /// <returns>A string with the comment's content with the replaced userIds with their corresponding usernames. For example if you have a pinged user with id 5c811dd9-8e68-4185-9fad-9eebda372bac and username pesho
        /// it will be stored in the database as @5c811dd9-8e68-4185-9fad-9eebda372bac. The front-end expects it as @pesho.</returns>
        Task<string> ReplaceUserIdsWithUsernames(string content);

        bool IsUserVotedForComment(string userId, Comment commentDb);
        void DeleteVote(Vote vote);

        List<Vote> GetVotes(Comment commentDb, Func<Vote, bool> filter = null, bool isQueryDeletedRecords = false);

        void SaveVote(Comment commentDb, Vote vote);
    }
}
