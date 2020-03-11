using DataAccess.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    public class User : IdentityUser, IBaseEntity // We extend the base IdentityUser in case we need to add more properties later
    {
        public ICollection<News> News { get; set; }
    }
}
