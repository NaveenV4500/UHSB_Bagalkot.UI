using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Data.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CropCategory> CropCategories { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<FarmerProfile> FarmersProfiles { get; set; }
        public DbSet<UserMaster> UserMasters { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CropCategory
             
            modelBuilder.Entity<CropCategory>()
    .HasKey(c => c.CategoryId);  // <-- define PK

            // Crop
            modelBuilder.Entity<Crop>()
                .HasOne(c => c.Category)
                .WithMany(cc => cc.Crops)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // FarmerProfile
            modelBuilder.Entity<FarmerProfile>()
                .HasIndex(f => f.Mobile)
                .IsUnique();
            
            modelBuilder.Entity<FarmerProfile>()
                .HasKey(f => f.FarmerId);

            modelBuilder.Entity<FarmerProfile>()
                .HasIndex(f => f.Mobile)
                .IsUnique();


            // UserMaster configuration
            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.HasIndex(u => u.PhoneNumber).IsUnique(); 
            });

            // Role configuration
            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.RoleName).IsUnique(); 
            });

            base.OnModelCreating(modelBuilder);
         

    }
}
}
