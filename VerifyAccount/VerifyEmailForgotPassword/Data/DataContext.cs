using Microsoft.EntityFrameworkCore;
using VerifyEmailForgotPassword.Models;

namespace VerifyEmailForgotPassword.Data
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.
                UseSqlServer("Data Source=.;Initial Catalog=UserDb;Persist Security Info=True;User Id=sa;Password=1;");
        }
        public DbSet<User> Users { get; set; }
    }
}
