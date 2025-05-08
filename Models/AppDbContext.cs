using Microsoft.EntityFrameworkCore;

namespace Proje.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Movie> Movies { get; set;}

        public DbSet<Comment> Comments { get; set; }
    }
}
