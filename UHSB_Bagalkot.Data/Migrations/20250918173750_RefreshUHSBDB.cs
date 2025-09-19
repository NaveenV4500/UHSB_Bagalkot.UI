using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UHSB_Bagalkot.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefreshUHSBDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarmersProfile",
                columns: table => new
                {
                    FarmerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mobile = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Village = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LandSize = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FarmersP__731B8888126DB797", x => x.FarmerId);
                });

            migrationBuilder.CreateTable(
                name: "UHSB_Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UHSB_Cat__19093A0B6CC0EDA7", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "UHSB_WeatherCastFileDetails",
                columns: table => new
                {
                    WeatherFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    WeekStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    WeekEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UHSB_WeatherCastFileDetails", x => x.WeatherFileId);
                });

            migrationBuilder.CreateTable(
                name: "UserMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    RoleType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserMast__3214EC07A35B5BF1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__3214EC079CB50282", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UHSB_Crops",
                columns: table => new
                {
                    CropId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UHSB_Cro__9235611589353871", x => x.CropId);
                    table.ForeignKey(
                        name: "FK_UHSB_Crops_UHSB_Category",
                        column: x => x.CategoryId,
                        principalTable: "UHSB_Categories",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RefreshT__3214EC07EBAB120B", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_UserMaster",
                        column: x => x.UserId,
                        principalTable: "UserMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UHSB_Sections",
                columns: table => new
                {
                    SectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CropId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UHSB_Sec__80EF087273D7BD88", x => x.SectionId);
                    table.ForeignKey(
                        name: "FK_UHSB_Sections_UHSB_Crop",
                        column: x => x.CropId,
                        principalTable: "UHSB_Crops",
                        principalColumn: "CropId");
                });

            migrationBuilder.CreateTable(
                name: "UHSB_SubSections",
                columns: table => new
                {
                    SubSectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UHSB_Sub__A8281A1DB2865AD6", x => x.SubSectionId);
                    table.ForeignKey(
                        name: "FK_UHSB_SubSections_UHSB_Section",
                        column: x => x.SectionId,
                        principalTable: "UHSB_Sections",
                        principalColumn: "SectionId");
                });

            migrationBuilder.CreateTable(
                name: "UHSB_ItemDeails",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubSectionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UHSB_Ite__727E838BC58DFEC2", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_UHSB_ItemDeails_UHSB_SubSection",
                        column: x => x.SubSectionId,
                        principalTable: "UHSB_SubSections",
                        principalColumn: "SubSectionId");
                });

            migrationBuilder.CreateTable(
                name: "ItemContent",
                columns: table => new
                {
                    ContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Article = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ItemCont__2907A81E44B946CF", x => x.ContentId);
                    table.ForeignKey(
                        name: "FK_UHSB_ItemContent_UHSB_Items",
                        column: x => x.ItemId,
                        principalTable: "UHSB_ItemDeails",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "UHSB_ItemImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UHSB_Ite__7516F70C28649EA1", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_UHSB_ItemImages_UHSB_Items",
                        column: x => x.ItemId,
                        principalTable: "UHSB_ItemDeails",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "UHSB_ItemQnA",
                columns: table => new
                {
                    QnAId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Datastatus = table.Column<byte>(type: "tinyint", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UHSB_Ite__C4DF8B097F677125", x => x.QnAId);
                    table.ForeignKey(
                        name: "FK_UHSB_ItemQnA_UHSB_Items",
                        column: x => x.ItemId,
                        principalTable: "UHSB_ItemDeails",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__FarmersP__6FAE0782B5B7AFAB",
                table: "FarmersProfile",
                column: "Mobile",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemContent_ItemId",
                table: "ItemContent",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UHSB_Crops_CategoryId",
                table: "UHSB_Crops",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UHSB_ItemDeails_SubSectionId",
                table: "UHSB_ItemDeails",
                column: "SubSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_UHSB_ItemImages_ItemId",
                table: "UHSB_ItemImages",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UHSB_ItemQnA_ItemId",
                table: "UHSB_ItemQnA",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UHSB_Sections_CropId",
                table: "UHSB_Sections",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_UHSB_SubSections_SectionId",
                table: "UHSB_SubSections",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "UQ__UserMast__85FB4E380FBF00DA",
                table: "UserMaster",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__UserRole__8A2B6160A53898F0",
                table: "UserRoles",
                column: "RoleName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmersProfile");

            migrationBuilder.DropTable(
                name: "ItemContent");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "UHSB_ItemImages");

            migrationBuilder.DropTable(
                name: "UHSB_ItemQnA");

            migrationBuilder.DropTable(
                name: "UHSB_WeatherCastFileDetails");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserMaster");

            migrationBuilder.DropTable(
                name: "UHSB_ItemDeails");

            migrationBuilder.DropTable(
                name: "UHSB_SubSections");

            migrationBuilder.DropTable(
                name: "UHSB_Sections");

            migrationBuilder.DropTable(
                name: "UHSB_Crops");

            migrationBuilder.DropTable(
                name: "UHSB_Categories");
        }
    }
}
