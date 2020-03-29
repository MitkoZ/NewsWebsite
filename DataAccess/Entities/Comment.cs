using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Comment : BaseEntity
    {
        public string ParentId { get; set; }
        //public string Created { get; set; }
        //public string Modified { get; set; }

        [Required]
        public string Content { get; set; }
        //public object Pings { get; set; } //TODO: do we really need this property? (it only keeps which users you pinged, but they are also contained in the content)
        public int UpvoteCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string NewsId { get; set; }

        public virtual News News { get; set; }
    }
}
