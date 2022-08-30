using Microsoft.EntityFrameworkCore;

namespace PasswordAppAPI.Models
{
#pragma warning disable IDE1006 // Naming Styles
    public partial class passwordAppContext : DbContext
#pragma warning restore IDE1006 // Naming Styles
    {
        public passwordAppContext()
        {
        }

        public passwordAppContext(DbContextOptions<passwordAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Password> Passwords { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=passwordAppDb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Password>(entity =>
            {
                entity.HasKey(e => e.PasswordsId)
                    .HasName("PK__PASSWORD__F19F32A53C75AC8D");

                entity.ToTable("PASSWORDS");

                entity.Property(e => e.PasswordsId).HasColumnName("PASSWORDS_ID");

                entity.Property(e => e.PasswordsPassword)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORDS_PASSWORD");

                entity.Property(e => e.PasswordsSite)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORDS_SITE");

                entity.Property(e => e.PasswordsUsername)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORDS_USERNAME");

                entity.Property(e => e.UserId).HasColumnName("USER_ID");

            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("USER_ID");

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasColumnName("INSERT_DATE");

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("USERNAME");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
