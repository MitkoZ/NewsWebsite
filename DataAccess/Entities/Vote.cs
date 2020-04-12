using DataAccess.Entities.Abstractions.Classes;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Vote : BaseBridgeEntity
    {
        [Required]
        public string CommentId { get; set; } // The comment id to which the vote belongs to
        public virtual Comment Comment { get; set; }

        [Required]
        public string UserId { get; set; } // The user (id) that made the vote for the comment
        public virtual User User { get; set; }
    }
}
