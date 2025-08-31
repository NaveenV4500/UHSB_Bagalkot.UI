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
        public DbSet<CropDetail> CropDetails { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

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
            modelBuilder.Entity<Crop>()
               .HasOne(c => c.Category)
               .WithMany(ca => ca.Crops)
               .HasForeignKey(c => c.CategoryId);

            modelBuilder.Entity<CropDetail>()
                .HasOne(cd => cd.Crop)
                .WithMany(c => c.CropDetails)
                .HasForeignKey(cd => cd.CropId);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Crop)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CropId);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.CreatedByUser)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.CreatedBy);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Crop)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CropId);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Crop)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CropId);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId);


            base.OnModelCreating(modelBuilder);

        }
    }
}
