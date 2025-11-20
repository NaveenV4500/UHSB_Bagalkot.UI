using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UHSB_Bagalkot.Data.Models;

public partial class Uhsb2025uatContext : DbContext
{
    public Uhsb2025uatContext()
    {
    }

    public Uhsb2025uatContext(DbContextOptions<Uhsb2025uatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FarmersProfile> FarmersProfiles { get; set; }

    public virtual DbSet<ItemContent> ItemContents { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<UhsbAvailabilityToolsDetail> UhsbAvailabilityToolsDetails { get; set; }

    public virtual DbSet<UhsbCategory> UhsbCategories { get; set; }

    public virtual DbSet<UhsbCrop> UhsbCrops { get; set; }

    public virtual DbSet<UhsbDistrict> UhsbDistricts { get; set; }

    public virtual DbSet<UhsbImageFile> UhsbImageFiles { get; set; }

    public virtual DbSet<UhsbItemDeail> UhsbItemDeails { get; set; }

    public virtual DbSet<UhsbItemImage> UhsbItemImages { get; set; }

    public virtual DbSet<UhsbItemQnA> UhsbItemQnAs { get; set; }

    public virtual DbSet<UhsbRecordHeadMaster> UhsbRecordHeadMasters { get; set; }

    public virtual DbSet<UhsbSection> UhsbSections { get; set; }

    public virtual DbSet<UhsbSectionsMapping> UhsbSectionsMappings { get; set; }

    public virtual DbSet<UhsbSeedPlantingCenterMaster> UhsbSeedPlantingCenterMasters { get; set; }

    public virtual DbSet<UhsbSubSection> UhsbSubSections { get; set; }

    public virtual DbSet<UhsbWeatherCastFileDetail> UhsbWeatherCastFileDetails { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
       /*UHSBGUIDE*/ //=> optionsBuilder.UseSqlServer("Server=D465PTN3;Database=UHSB_GUIDEUAT;Trusted_Connection=True;TrustServerCertificate=True;");
        => optionsBuilder.UseSqlServer("Server=DESKTOP-5GU02OK;Database=UHSB2025UAT;Trusted_Connection=True;TrustServerCertificate=True;");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FarmersProfile>(entity =>
        {
            entity.HasKey(e => e.FarmerId).HasName("PK__FarmersP__731B8888DEB72E8B");

            entity.ToTable("FarmersProfile");

            entity.HasIndex(e => e.Mobile, "UQ__FarmersP__6FAE07828D41B39D").IsUnique();

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
            entity.HasKey(e => e.ContentId).HasName("PK__ItemCont__2907A81E2BF99941");

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
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC070CF19983");

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Expires).HasColumnType("datetime");
            entity.Property(e => e.Revoked).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_UserMaster");
        });

        modelBuilder.Entity<UhsbAvailabilityToolsDetail>(entity =>
        {
            entity.HasKey(e => e.Identifier);

            entity.ToTable("UHSB_AvailabilityToolsDetails");

            entity.Property(e => e.Identifier).HasColumnName("identifier");
            entity.Property(e => e.AvailToolnameEng)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AvailToolname_eng");
            entity.Property(e => e.AvailToolnameKnd)
                .HasMaxLength(200)
                .HasColumnName("AvailToolname_knd");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Remarks)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Center).WithMany(p => p.UhsbAvailabilityToolsDetails)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_AvailabilityToolsDetails_UHSB_SeedPlantingCenterMaster");

            entity.HasOne(d => d.Head).WithMany(p => p.UhsbAvailabilityToolsDetails)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_AvailabilityToolsDetails_UHSB_RecordHeadMaster");
        });

        modelBuilder.Entity<UhsbCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__UHSB_Cat__19093A0B2E61F51B");

            entity.ToTable("UHSB_Categories");

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<UhsbCrop>(entity =>
        {
            entity.HasKey(e => e.CropId).HasName("PK__UHSB_Cro__9235611535F4CEC3");

            entity.ToTable("UHSB_Crops");

            entity.Property(e => e.CropId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.UhsbCrops)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_Crops_UHSB_Category");
        });

        modelBuilder.Entity<UhsbDistrict>(entity =>
        {
            entity.HasKey(e => e.DistrictId).HasName("PK__UHSB_Dis__85FDA4C6C73B8145");

            entity.ToTable("UHSB_Districts");

            entity.Property(e => e.DistrictId).ValueGeneratedNever();
            entity.Property(e => e.DistrictName).HasMaxLength(100);
        });

        modelBuilder.Entity<UhsbImageFile>(entity =>
        {
            entity.HasKey(e => e.FileId);

            entity.ToTable("UHSB_ImageFiles");

            entity.Property(e => e.FilePath)
                .HasMaxLength(400)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UhsbItemDeail>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__UHSB_Ite__727E838BB9B75887");

            entity.ToTable("UHSB_ItemDeails");

            entity.Property(e => e.ItemId).ValueGeneratedNever();
            entity.Property(e => e.ImageUrl).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.SectionMap).WithMany(p => p.UhsbItemDeails)
                .HasForeignKey(d => d.SectionMapId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_ItemDeails_UHSB_SectionsMapping");
        });

        modelBuilder.Entity<UhsbItemImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__UHSB_Ite__7516F70CC194DC00");

            entity.ToTable("UHSB_ItemImages");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Item).WithMany(p => p.UhsbItemImages)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_ItemImages_UHSB_Items");
        });

        modelBuilder.Entity<UhsbItemQnA>(entity =>
        {
            entity.HasKey(e => e.QnAid).HasName("PK__UHSB_Ite__C4DF8B094A82A38F");

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

        modelBuilder.Entity<UhsbRecordHeadMaster>(entity =>
        {
            entity.HasKey(e => e.HeadId).HasName("PK__UHSB_Rec__EB3F25101021E532");

            entity.ToTable("UHSB_RecordHeadMaster");

            entity.Property(e => e.HeadId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RecordHeadEng)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("RecordHead_eng");
            entity.Property(e => e.RecordHeadKnd)
                .HasMaxLength(200)
                .HasColumnName("RecordHead_knd");
        });

        modelBuilder.Entity<UhsbSection>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__UHSB_Sec__80EF0872789D59F0");

            entity.ToTable("UHSB_Sections");

            entity.Property(e => e.SectionId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Crop).WithMany(p => p.UhsbSections)
                .HasForeignKey(d => d.CropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHSB_Sections_UHSB_Crop");
        });

        modelBuilder.Entity<UhsbSectionsMapping>(entity =>
        {
            entity.HasKey(e => e.SectionMapId).HasName("PK__UHSB_Sec__8387823043608A43");

            entity.ToTable("UHSB_SectionsMapping");

            entity.Property(e => e.SectionMapId).ValueGeneratedNever();
        });

        modelBuilder.Entity<UhsbSeedPlantingCenterMaster>(entity =>
        {
            entity.HasKey(e => e.CenterId).HasName("PK__UHSB_See__398FC7F74970FCE3");

            entity.ToTable("UHSB_SeedPlantingCenterMaster");

            entity.Property(e => e.CenterId).ValueGeneratedNever();
            entity.Property(e => e.CenternameEng)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Centername_eng");
            entity.Property(e => e.CenternameKnd)
                .HasMaxLength(200)
                .HasColumnName("Centername_knd");

            entity.HasOne(d => d.District).WithMany(p => p.UhsbSeedPlantingCenterMasters)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_UHSB_SeedPlantingCenterMaster_Districts");
        });

        modelBuilder.Entity<UhsbSubSection>(entity =>
        {
            entity.HasKey(e => e.SubSectionId).HasName("PK__UHSB_Sub__A8281A1DF506708B");

            entity.ToTable("UHSB_SubSections");

            entity.Property(e => e.SubSectionId).ValueGeneratedNever();
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

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserMast__3214EC0794D52E0D");

            entity.ToTable("UserMaster");

            entity.HasIndex(e => e.PhoneNumber, "UQ__UserMast__85FB4E384B296982").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(150)
                .HasColumnName("EmailID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LandSize).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.Village).HasMaxLength(250);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC0777FBBBE5");

            entity.HasIndex(e => e.RoleName, "UQ__UserRole__8A2B6160008193FC").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
