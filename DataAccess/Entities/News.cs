﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class News : BaseEntity
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
    }
}
