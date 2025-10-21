using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using My_books.Data.Models;

namespace My_books
{
    public class ProjectDbContext : IdentityDbContext<ApplicationUser>
    {
        #region [-Ctor-]
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {
        }
        #endregion

        #region [-DbSets-]
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book_Author> Book_Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet< RefreshToken> RefreshTokens { get; set; }

        #endregion

        #region [-OnModelCreating-]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book_Author>()
                .HasOne(b => b.Book)
                .WithMany(ba => ba.Book_Authors)
                .HasForeignKey(bi => bi.BookId);

            modelBuilder.Entity<Book_Author>()
                .HasOne(b => b.Author)
                .WithMany(ba => ba.Book_Authors)
                .HasForeignKey(bi => bi.AuthorId);
            
                
           base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
