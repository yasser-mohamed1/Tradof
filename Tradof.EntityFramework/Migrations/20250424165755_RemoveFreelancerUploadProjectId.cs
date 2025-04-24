using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFreelancerUploadProjectId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Projects_FreelancerUploadProjectId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_FreelancerUploadProjectId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FreelancerUploadProjectId",
                table: "Files");

            migrationBuilder.AddColumn<bool>(
                name: "IsFreelancerUpload",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFreelancerUpload",
                table: "Files");

            migrationBuilder.AddColumn<long>(
                name: "FreelancerUploadProjectId",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_FreelancerUploadProjectId",
                table: "Files",
                column: "FreelancerUploadProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Projects_FreelancerUploadProjectId",
                table: "Files",
                column: "FreelancerUploadProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
