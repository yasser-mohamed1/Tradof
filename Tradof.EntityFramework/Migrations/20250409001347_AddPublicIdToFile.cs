using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Files");
        }
    }
}
