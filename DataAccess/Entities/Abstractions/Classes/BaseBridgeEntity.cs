using DataAccess.Entities.Abstractions.Interfaces;

namespace DataAccess.Entities.Abstractions.Classes
{
    public abstract class BaseBridgeEntity : BaseEntity, IBaseBridgeEntity // We create a separate class for the bridge entities in case we want to add special properties for them
    {
    }
}
