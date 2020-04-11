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

            modelBuilder.Entity<Vote>()
                .HasOne(vote => vote.Comment)
                .WithMany(comment => comment.Votes)
                .HasForeignKey(vote => vote.CommentId);

            modelBuilder.Entity<Vote>()
                .HasOne(vote => vote.User)
                .WithMany(user => user.Votes)
                .HasForeignKey(user => user.UserId);

            modelBuilder
                .Entity<Vote>()
                .HasKey(vote => new { vote.CommentId, vote.UserId });

            modelBuilder.RemoveCascadeDelete();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}
