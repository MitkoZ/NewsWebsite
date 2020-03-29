using DataAccess.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
