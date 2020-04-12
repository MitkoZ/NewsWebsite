using DataAccess.Entities.Abstractions.Interfaces;

namespace DataAccess.Entities.Abstractions.Classes
{
    public abstract class BaseEntity : IBaseEntity
    {
        public bool IsDeleted { get; set; }
    }
}
