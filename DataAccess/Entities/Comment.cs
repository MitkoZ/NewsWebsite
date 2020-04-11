using DataAccess.Entities.Abstractions.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Comment : BaseNormalEntity
    {
        public string ParentId { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string NewsId { get; set; }

        public virtual News News { get; set; }

        public virtual List<Vote> Votes { get; set; }

        public Comment()
        {
            this.Votes = new List<Vote>();
        }
    }
}
