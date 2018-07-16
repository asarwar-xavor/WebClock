using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace GC.WebClock.Models
{
    public partial class GC_WebClockContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<ClocksList> ClocksList { get; set; }
        public virtual DbSet<ConfigurationProperties> ConfigurationProperties { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public GC_WebClockContext(DbContextOptions options): base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClocksList>(entity =>
            {
                entity.HasKey(e => e.ClockId)
                    .HasName("PK_ClocksList");

                entity.Property(e => e.ClockName).HasMaxLength(500);
               // entity.Property(e => e.LocationName).HasMaxLength(500);
                entity.Property(e => e.Location).HasMaxLength(500);
                entity.Property(e => e.ClockType).HasMaxLength(50);
                entity.Property(e => e.EncodedSecurityCode).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.IsActive).HasDefaultValueSql("1");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConfigurationProperties>(entity =>
            {
                entity.HasKey(e => e.PropertyId)
                    .HasName("PK_ConfigurationProperties");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.IsActive).HasDefaultValueSql("1");

                entity.Property(e => e.ModifiedBy).HasMaxLength(255);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PropertyName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PropertyValue)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Description).HasMaxLength(255);
            });

            modelBuilder.Entity<DataProtectionKey>(entity =>
            {
                entity.HasKey(e => e.DataProtectionKeysId).HasName("DataProtectionKeysId");
                entity.Property(e => e.AuthKey).HasMaxLength(255);
                entity.Property(e => e.XmlData).HasMaxLength(255);
            });

        }
    }
}