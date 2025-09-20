using Microsoft.EntityFrameworkCore;
using My_books.Data.Models;

namespace My_books
{
    public class ProjectDbContext:DbContext
    {
        public ProjectDbContext(DbContextOptions options) : base(options)
        {
        }

        //db sets
        public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }



        }
}
