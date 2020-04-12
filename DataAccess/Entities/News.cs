using DataAccess.Entities.Abstractions.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class News : BaseNormalEntity // News doesn't have a singular
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; } // we save it in Deltas format (subset of JSON, specific for the Quill WYSIWYG editor)

        [Required]
        public string UserId { get; set; }

        [Required]
        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
