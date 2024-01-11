using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppMvc.Net.Migrations
{
    public partial class Skill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionSkill");

            migrationBuilder.DropTable(
                name: "ProfessionSkillCategory");

            migrationBuilder.CreateTable(
                name: "CategorySkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentCategorySkillId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategorySkill_CategorySkill_ParentCategorySkillId",
                        column: x => x.ParentCategorySkillId,
                        principalTable: "CategorySkill",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.SkillId);
                    table.ForeignKey(
                        name: "FK_Skill_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SkillCategorySkill",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCategorySkill", x => new { x.SkillID, x.CategoryID });
                    table.ForeignKey(
                        name: "FK_SkillCategorySkill_CategorySkill_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "CategorySkill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillCategorySkill_Skill_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skill",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategorySkill_ParentCategorySkillId",
                table: "CategorySkill",
                column: "ParentCategorySkillId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySkill_Slug",
                table: "CategorySkill",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skill_AuthorId",
                table: "Skill",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_Slug",
                table: "Skill",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SkillCategorySkill_CategoryID",
                table: "SkillCategorySkill",
                column: "CategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillCategorySkill");

            migrationBuilder.DropTable(
                name: "CategorySkill");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.CreateTable(
                name: "ProfessionSkillCategory",
                columns: table => new
                {
                    ProfessionSkillCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionSkillCategory", x => x.ProfessionSkillCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "ProfessionSkill",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionSkillCategoryID = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionSkill", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProfessionSkill_ProfessionSkillCategory_ProfessionSkillCategoryID",
                        column: x => x.ProfessionSkillCategoryID,
                        principalTable: "ProfessionSkillCategory",
                        principalColumn: "ProfessionSkillCategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionSkill_ProfessionSkillCategoryID",
                table: "ProfessionSkill",
                column: "ProfessionSkillCategoryID");
        }
    }
}
