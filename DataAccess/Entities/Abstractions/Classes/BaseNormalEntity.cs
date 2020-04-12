using DataAccess.Entities.Abstractions.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Abstractions.Classes
{
    public abstract class BaseNormalEntity : BaseEntity, IBaseNormalEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
    }
}
