using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MakeRatingValueDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "RatingValue",
                table: "Ratings",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RatingValue",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
