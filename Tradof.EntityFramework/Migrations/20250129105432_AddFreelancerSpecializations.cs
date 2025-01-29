using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddFreelancerSpecializations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Freelancers_Specializations_SpecializationId",
                table: "Freelancers");

            migrationBuilder.DropIndex(
                name: "IX_Freelancers_SpecializationId",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Freelancers");

            migrationBuilder.CreateTable(
                name: "FreelancerSpecialization",
                columns: table => new
                {
                    FreelancersId = table.Column<long>(type: "bigint", nullable: false),
                    SpecializationsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreelancerSpecialization", x => new { x.FreelancersId, x.SpecializationsId });
                    table.ForeignKey(
                        name: "FK_FreelancerSpecialization_Freelancers_FreelancersId",
                        column: x => x.FreelancersId,
                        principalTable: "Freelancers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FreelancerSpecialization_Specializations_SpecializationsId",
                        column: x => x.SpecializationsId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerSpecialization_SpecializationsId",
                table: "FreelancerSpecialization",
                column: "SpecializationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FreelancerSpecialization");

            migrationBuilder.AddColumn<long>(
                name: "SpecializationId",
                table: "Freelancers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Freelancers_SpecializationId",
                table: "Freelancers",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Freelancers_Specializations_SpecializationId",
                table: "Freelancers",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
