using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddFreelancerLanguagesPairExamResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Free_HasTakenExam",
                table: "FreelancerLanguagesPairs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Free_Mark",
                table: "FreelancerLanguagesPairs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Pro1_HasTakenExam",
                table: "FreelancerLanguagesPairs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pro1_Mark",
                table: "FreelancerLanguagesPairs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Pro2_HasTakenExam",
                table: "FreelancerLanguagesPairs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pro2_Mark",
                table: "FreelancerLanguagesPairs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Free_HasTakenExam",
                table: "FreelancerLanguagesPairs");

            migrationBuilder.DropColumn(
                name: "Free_Mark",
                table: "FreelancerLanguagesPairs");

            migrationBuilder.DropColumn(
                name: "Pro1_HasTakenExam",
                table: "FreelancerLanguagesPairs");

            migrationBuilder.DropColumn(
                name: "Pro1_Mark",
                table: "FreelancerLanguagesPairs");

            migrationBuilder.DropColumn(
                name: "Pro2_HasTakenExam",
                table: "FreelancerLanguagesPairs");

            migrationBuilder.DropColumn(
                name: "Pro2_Mark",
                table: "FreelancerLanguagesPairs");
        }
    }
}
