namespace DataAccess.Entities.Abstractions.Interfaces
{
    public interface IBaseNormalEntity : IBaseEntity
    {
        public string Id { get; set; } // We use string and not guid for flexibility (not all database providers have the guid type) - http://stackoverflow.com/questions/32134589/why-did-the-new-asp-net-identity-tables-stop-using-guid-uniqueidentifier-type
    }
}
