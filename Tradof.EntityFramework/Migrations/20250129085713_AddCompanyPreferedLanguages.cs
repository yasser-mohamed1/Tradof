using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyPreferedLanguages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Specializations_SpecializationId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_SpecializationId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Companies");

            migrationBuilder.CreateTable(
                name: "CompanySpecialization",
                columns: table => new
                {
                    CompaniesId = table.Column<long>(type: "bigint", nullable: false),
                    SpecializationsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySpecialization", x => new { x.CompaniesId, x.SpecializationsId });
                    table.ForeignKey(
                        name: "FK_CompanySpecialization_Companies_CompaniesId",
                        column: x => x.CompaniesId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanySpecialization_Specializations_SpecializationsId",
                        column: x => x.SpecializationsId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanySpecialization_SpecializationsId",
                table: "CompanySpecialization",
                column: "SpecializationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySpecialization");

            migrationBuilder.AddColumn<long>(
                name: "SpecializationId",
                table: "Companies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_SpecializationId",
                table: "Companies",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Specializations_SpecializationId",
                table: "Companies",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
