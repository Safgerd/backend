using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using WebApp.Data.Models;

namespace WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Настройка связи Post -> Comments (один ко многим)
            builder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // или Restrict, если не хотите каскадного удаления

            // Настройка связи User -> Comments (один ко многим)
            // IdentityUser уже имеет коллекцию, EF Core обычно сам справляется
            // Но для явности можно:
            builder.Entity<IdentityUser>()
                .HasMany<Comment>() // Указываем тип сущности, если у IdentityUser нет явного свойства ICollection<Comment>
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Обычно не удаляем пользователя при удалении его комментария

            // Связь User -> Posts
            builder.Entity<IdentityUser>()
               .HasMany<Post>()
               .WithOne(p => p.User)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);
        }

        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
    }
}
