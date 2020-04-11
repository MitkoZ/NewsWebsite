using DataAccess.Entities.Abstractions.Classes;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Vote : BaseBridgeEntity
    {
        [Required]
        public string CommentId { get; set; }
        public virtual Comment Comment { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
