using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UHSB_Bagalkot.Data.Models;

public partial class Uhsb2025Context : DbContext
{
    public Uhsb2025Context()
    {
    }

    public Uhsb2025Context(DbContextOptions<Uhsb2025Context> options)
        : base(options)
    {
    }

    public virtual DbSet<FarmersProfile> FarmersProfiles { get; set; }

    public virtual DbSet<ItemContent> ItemContents { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<UhsbCategory> UhsbCategories { get; set; }

    public virtual DbSet<UhsbCrop> UhsbCrops { get; set; }

    public virtual DbSet<UhsbDistrict> UhsbDistricts { get; set; }

    public virtual DbSet<UhsbItemDeail> UhsbItemDeails { get; set; }

    public virtual DbSet<UhsbItemImage> UhsbItemImages { get; set; }

    public virtual DbSet<UhsbItemQnA> UhsbItemQnAs { get; set; }

    public virtual DbSet<UhsbSection> UhsbSections { get; set; }

    public virtual DbSet<UhsbSubSection> UhsbSubSections { get; set; }

    public virtual DbSet<UhsbWeatherCastFileDetail> UhsbWeatherCastFileDetails { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }
    public DbSet<SPWeeklyWeatherRecord> SP_WeeklyWeatherRecords { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-5GU02OK;Database=UHSB2025;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.Entity<SPWeeklyWeatherRecord>(entity =>
        {
            entity.HasNoKey(); // keyless entity
            entity.ToView(null); // not mapped to a table

            // DateOnly conversion if needed
            entity.Property(e => e.WeekStartDate)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null
                );

            entity.Property(e => e.WeekEndDate)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null
                );

            entity.Property(e => e.CreatedDate)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null
                );
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

        modelBuilder.Entity<ItemContent>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__ItemCont__2907A81E44B946CF");

            entity.ToTable("ItemContent");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Item).WithMany(p => p.ItemContents)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_ItemContent_UHSB_Items");
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

        modelBuilder.Entity<UhsbCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__UHSB_Cat__19093A0B6CC0EDA7");

            entity.ToTable("UHSB_Categories");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<UhsbCrop>(entity =>
        {
            entity.HasKey(e => e.CropId).HasName("PK__UHSB_Cro__9235611589353871");

            entity.ToTable("UHSB_Crops");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.UhsbCrops)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_Crops_UHSB_Category");
        });

        modelBuilder.Entity<UhsbDistrict>(entity =>
        {
            entity.HasKey(e => e.DistrictId).HasName("PK__UHSB_Dis__85FDA4C68A83E225");

            entity.ToTable("UHSB_Districts");

            entity.Property(e => e.DistrictName).HasMaxLength(100);
        });

        modelBuilder.Entity<UhsbItemDeail>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__UHSB_Ite__727E838BC58DFEC2");

            entity.ToTable("UHSB_ItemDeails");

            entity.Property(e => e.ImageUrl).IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.SubSection).WithMany(p => p.UhsbItemDeails)
                .HasForeignKey(d => d.SubSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_ItemDeails_UHSB_SubSection");
        });

        modelBuilder.Entity<UhsbItemImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__UHSB_Ite__7516F70C28649EA1");

            entity.ToTable("UHSB_ItemImages");

            entity.Property(e => e.ImageUrl).IsUnicode(false);

            entity.HasOne(d => d.Item).WithMany(p => p.UhsbItemImages)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_ItemImages_UHSB_Items");
        });

        modelBuilder.Entity<UhsbItemQnA>(entity =>
        {
            entity.HasKey(e => e.QnAid).HasName("PK__UHSB_Ite__C4DF8B097F677125");

            entity.ToTable("UHSB_ItemQnA");

            entity.Property(e => e.QnAid).HasColumnName("QnAId");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Item).WithMany(p => p.UhsbItemQnAs)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_ItemQnA_UHSB_Items");
        });

        modelBuilder.Entity<UhsbSection>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__UHSB_Sec__80EF087273D7BD88");

            entity.ToTable("UHSB_Sections");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Crop).WithMany(p => p.UhsbSections)
                .HasForeignKey(d => d.CropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_Sections_UHSB_Crop");
        });

        modelBuilder.Entity<UhsbSubSection>(entity =>
        {
            entity.HasKey(e => e.SubSectionId).HasName("PK__UHSB_Sub__A8281A1DB2865AD6");

            entity.ToTable("UHSB_SubSections");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Section).WithMany(p => p.UhsbSubSections)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_SubSections_UHSB_Section");
        });

        modelBuilder.Entity<UhsbWeatherCastFileDetail>(entity =>
        {
            entity.HasKey(e => e.WeatherFileId);

            entity.ToTable("UHSB_WeatherCastFileDetails");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FilePath).HasMaxLength(500);

            entity.HasOne(d => d.District).WithMany(p => p.UhsbWeatherCastFileDetails)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_WeatherCastFileDetails_Districts");

            entity.HasOne(d => d.User).WithMany(p => p.UhsbWeatherCastFileDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UHSB_WeatherCastFileDetails_UserMaster");
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
