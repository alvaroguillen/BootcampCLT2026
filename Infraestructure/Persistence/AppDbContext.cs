using BootcampCLT2026.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace BootcampCLT2026.Infraestructure.Persistence
{
    public class    AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Cuenta> Cuentas => Set<Cuenta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.ToTable("accounts");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.NumeroCuenta)
                    .HasColumnName("account_number")
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(a => a.NombreTitular)
                    .HasColumnName("holder_name")
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(a => a.Saldo)
                    .HasColumnName("balance")
                    .HasColumnType("numeric(18,2)")
                    .IsRequired();

                entity.Property(a => a.Estado)
                    .HasColumnName("status")
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(a => a.FechaCreacion)
                    .HasColumnName("created_at")
                    .IsRequired()
                    .HasConversion(
                        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

                entity.HasIndex(a => a.NumeroCuenta)
                    .IsUnique()
                    .HasDatabaseName("ix_accounts_account_number");
            });
        }
    }
}
