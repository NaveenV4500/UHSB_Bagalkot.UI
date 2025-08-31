using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UHSB_Bagalkot.UI.Models;

namespace UHSB_Bagalkot.UI.Data;

public partial class UHSBContext : DbContext
{
    public UHSBContext()
    {
    }

    public UHSBContext(DbContextOptions<UHSBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleItem> ArticleItems { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<CropCategory> CropCategories { get; set; }

    public virtual DbSet<CropDetail> CropDetails { get; set; }

    public virtual DbSet<FarmersProfile> FarmersProfiles { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270E8197B9D43");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CropDetailId).HasColumnName("CropDetailID");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Articles)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Articles__Create__03F0984C");

            entity.HasOne(d => d.CropDetail).WithMany(p => p.Articles)
                .HasForeignKey(d => d.CropDetailId)
                .HasConstraintName("FK_Articles_CropDetails");

            entity.HasOne(d => d.Crop).WithMany(p => p.Articles)
                .HasForeignKey(d => d.CropId)
                .HasConstraintName("FK__Articles__CropId__02FC7413");
        });

        modelBuilder.Entity<ArticleItem>(entity =>
        {
            entity.HasKey(e => e.ArticleItemsId).HasName("PK__ArticleI__8D1D7245D034F5A7");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleItems)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__ArticleIt__Artic__08B54D69");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ArticleItems)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ArticleIt__Creat__09A971A2");

            entity.HasOne(d => d.Crop).WithMany(p => p.ArticleItems)
                .HasForeignKey(d => d.CropId)
                .HasConstraintName("FK__ArticleIt__CropI__07C12930");
        });

        modelBuilder.Entity<Crop>(entity =>
        {
            entity.HasKey(e => e.CropId).HasName("PK__Crops__9235611587DEECB3");

            entity.Property(e => e.CropName).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Crops)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Crops__CategoryI__6FE99F9F");
        });

        modelBuilder.Entity<CropCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__CropCate__19093A0BB0F69E67");

            entity.HasIndex(e => e.CategoryName, "UQ__CropCate__8517B2E088894268").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(250);
        });

        modelBuilder.Entity<CropDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__CropDeta__135C316DC3B91F42");

            entity.Property(e => e.DetailType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Crop).WithMany(p => p.CropDetails)
                .HasForeignKey(d => d.CropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CropDetai__CropI__72C60C4A");
        });

        modelBuilder.Entity<FarmersProfile>(entity =>
        {
            entity.HasKey(e => e.FarmerId).HasName("PK__FarmersP__731B8888126DB797");

            entity.ToTable("FarmersProfile");

            entity.HasIndex(e => e.Mobile, "UQ__FarmersP__6FAE0782B5B7AFAB").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LandSize).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Village).HasMaxLength(100);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07EBAB120B");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Expires).HasColumnType("datetime");
            entity.Property(e => e.Revoked).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_UserMaster");
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserMast__3214EC07A35B5BF1");

            entity.ToTable("UserMaster");

            entity.HasIndex(e => e.PhoneNumber, "UQ__UserMast__85FB4E380FBF00DA").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC079CB50282");

            entity.HasIndex(e => e.RoleName, "UQ__UserRole__8A2B6160A53898F0").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
