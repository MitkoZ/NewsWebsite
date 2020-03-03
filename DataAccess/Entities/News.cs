﻿using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class News : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; } // we save it in Deltas format (subset of JSON, specific for the Quill WYSIWYG editor)
    }
}