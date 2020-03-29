﻿using DataAccess.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    public class User : IdentityUser, IBaseEntity // We extend the base IdentityUser in case we need to add more properties later
    {
        public bool IsDeleted { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
