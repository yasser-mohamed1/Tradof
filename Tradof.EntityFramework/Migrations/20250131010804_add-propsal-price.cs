using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class addpropsalprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OfferPrice",
                table: "Proposals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferPrice",
                table: "Proposals");
        }
    }
}
