using System.Threading.Tasks;

namespace Services.Auth.Interfaces
{
    public interface IItemLookup
    {
        /// <summary>
        /// Gets the user id to who the resource belongs to
        /// </summary>
        /// <param name="itemId">The id of the resource</param>
        /// <returns>The id of the user to who the resource belongs to</returns>
        Task<string> GetOwnerId(string itemId);
    }
}
