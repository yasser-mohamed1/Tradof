using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Languages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CompanyId",
                table: "Languages",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Companies_CompanyId",
                table: "Languages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Companies_CompanyId",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_CompanyId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Languages");
        }
    }
}
