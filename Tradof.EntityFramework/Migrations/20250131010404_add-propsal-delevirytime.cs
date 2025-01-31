using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class addpropsaldelevirytime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjecDeliveryTime",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjecDeliveryTime",
                table: "Proposals");
        }
    }
}
