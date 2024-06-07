using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pdfyai_api.Models;

namespace pdfyai_api.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);




            builder.HasDefaultSchema("pdfyai");
            builder.Entity<Chat>()
            .HasOne(c => c.User);

            builder.Entity<Chat>()
            .HasOne(c => c.Document);

            builder.Entity<Chat>()
            .HasMany(c => c.Messages);

            builder.Entity<Document>()
            .HasOne(d => d.User);

            builder.Entity<Payment>()
            .HasOne(p => p.User);





        }


        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Document> Documents { get; set; }

        public DbSet<Payment> Payments { get; set; }
    }
}