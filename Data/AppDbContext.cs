using demodockerv2.webapi.Models;
using Microsoft.EntityFrameworkCore;

namespace demodockerv2.webapi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

   
        public DbSet<Student> Students { get; set; }
    }
}
