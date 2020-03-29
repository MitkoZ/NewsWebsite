using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Utils
{
    internal static class ModelBuilderExtensions
    {
        internal static void RemoveCascadeDelete(this ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> cascadeForeignKeys = modelBuilder.Model.GetEntityTypes()
                                               .SelectMany(t => t.GetForeignKeys())
                                               .Where(foreignKey => !foreignKey.IsOwnership && foreignKey.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (IMutableForeignKey foreignKey in cascadeForeignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
