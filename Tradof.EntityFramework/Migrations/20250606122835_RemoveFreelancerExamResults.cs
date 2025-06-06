using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFreelancerExamResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Free_HasTakenExam",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "Free_Mark",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "Pro1_HasTakenExam",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "Pro1_Mark",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "Pro2_HasTakenExam",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "Pro2_Mark",
                table: "Freelancers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Free_HasTakenExam",
                table: "Freelancers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Free_Mark",
                table: "Freelancers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Pro1_HasTakenExam",
                table: "Freelancers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pro1_Mark",
                table: "Freelancers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Pro2_HasTakenExam",
                table: "Freelancers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pro2_Mark",
                table: "Freelancers",
                type: "int",
                nullable: true);
        }
    }
}
