using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebRecommend.Models;

namespace WebRecommend.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleTag>()
                .HasKey(c => new { c.ArticleId, c.TagId });
            modelBuilder.Entity<Rating>()
                .HasKey(c => new { c.ArticleId, c.UserId });
            modelBuilder.Entity<Reputation>()
                .HasKey(c => new { c.ArticleId, c.UserId });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AppUser> ApplicationUsers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleTag> ArticleTags { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Rating> Rating { get; set; }

        public DbSet<Reputation> Reputation { get; set; }

        public DbSet<Comment> Comments { get; set; }

    }
}
