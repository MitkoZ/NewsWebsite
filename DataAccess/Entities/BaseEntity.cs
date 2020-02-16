using DataAccess.Entities.Interfaces;

namespace DataAccess.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        public string Id { get; set; }
    }
}
