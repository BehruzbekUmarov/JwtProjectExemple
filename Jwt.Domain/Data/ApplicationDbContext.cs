using Jwt.WebUI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jwt.WebUI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        {   
        }

        public DbSet<User> Users { get; set; }
    }
}
