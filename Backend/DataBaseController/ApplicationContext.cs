using Microsoft.EntityFrameworkCore;
using Backend.Models;
namespace Backend.DataBaseController
{
    public class ApplicationContext : DbContext
    {
        public DbSet<AccountUser> Users { get; set; } = null!;
        public DbSet<UserAnswerTask> Answers { get; set; } = null!;
        public DbSet<UserAnswerTest> TestsUsers { get; set; } = null!;
        public DbSet<QuestionsComponent> Questions { get; set; } = null!;
        public DbSet<TestModule> moduleTest { get; set; } = null!;
        public DbSet<RequestUser> RequestUsers { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
        }
    }
}
