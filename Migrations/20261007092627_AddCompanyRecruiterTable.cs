using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PivorkJobportal.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRecruiterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "CompanyProfiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "JobPostings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "JobPostings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyRecruiters",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyProfileId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsCompanyAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsApprovedByCompany = table.Column<bool>(type: "bit", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRecruiters", x => new { x.UserId, x.CompanyProfileId });
                    table.ForeignKey(
                        name: "FK_CompanyRecruiters_CompanyProfiles_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "CompanyProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyRecruiters_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_OwnerId",
                table: "JobPostings",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRecruiters_CompanyProfileId",
                table: "CompanyRecruiters",
                column: "CompanyProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_User_OwnerId",
                table: "JobPostings",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_User_OwnerId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "CompanyRecruiters");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_OwnerId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "JobPostings");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "CompanyProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
