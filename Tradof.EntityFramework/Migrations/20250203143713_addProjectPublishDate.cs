using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class addProjectPublishDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Freelancers_FreelancerId",
                table: "PaymentMethods");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Freelancers_FreelancerId",
                table: "PaymentMethods",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Freelancers_FreelancerId",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Freelancers_FreelancerId",
                table: "PaymentMethods",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id");
        }
    }
}
