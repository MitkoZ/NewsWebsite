using DataAccess.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class User : IdentityUser, IBaseEntity // We extend the base IdentityUser in case we need to add more properties later
    {
    }
}
