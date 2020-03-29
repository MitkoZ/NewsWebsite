using DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataAccess.Utils;

namespace DataAccess
{
    public class NewsDbContext : IdentityDbContext<User>
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<News>()
                .HasIndex(news => news.Title).IsUnique();

            modelBuilder.RemoveCascadeDelete();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
