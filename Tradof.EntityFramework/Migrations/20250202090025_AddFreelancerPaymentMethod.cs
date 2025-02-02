using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddFreelancerPaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerLanguagesPairs_Freelancers_FreelancerId",
                table: "FreelancerLanguagesPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_Freelancers_AspNetUsers_UserId",
                table: "Freelancers");

            migrationBuilder.DropForeignKey(
                name: "FK_Freelancers_Countries_CountryId",
                table: "Freelancers");

            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerSocialMedias_Freelancers_FreelancerId",
                table: "FreelancerSocialMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerSpecialization_Freelancers_FreelancersId",
                table: "FreelancerSpecialization");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentProcess_Freelancers_FreelancerId",
                table: "PaymentProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Freelancers_FreelancerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Freelancers_FreelancerId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_WorksOn_Freelancers_FreelancerId",
                table: "WorksOn");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Freelancers",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "ProjectPayment");

            migrationBuilder.RenameTable(
                name: "Freelancers",
                newName: "Freelancer");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancers_UserId",
                table: "Freelancer",
                newName: "IX_Freelancer_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancers_CountryId",
                table: "Freelancer",
                newName: "IX_Freelancer_CountryId");

            migrationBuilder.AlterColumn<long>(
                name: "SpecializationId",
                table: "Projects",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Projects",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "FreelancerId",
                table: "Projects",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "PaymentMethodId",
                table: "ProjectPayment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CVFilePath",
                table: "Freelancer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Freelancer",
                table: "Freelancer",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CVV = table.Column<int>(type: "int", nullable: false),
                    FreelancerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethod_Freelancer_FreelancerId",
                        column: x => x.FreelancerId,
                        principalTable: "Freelancer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPayment_PaymentMethodId",
                table: "ProjectPayment",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_FreelancerId",
                table: "PaymentMethod",
                column: "FreelancerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Freelancer_AspNetUsers_UserId",
                table: "Freelancer",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Freelancer_Countries_CountryId",
                table: "Freelancer",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerLanguagesPairs_Freelancer_FreelancerId",
                table: "FreelancerLanguagesPairs",
                column: "FreelancerId",
                principalTable: "Freelancer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerSocialMedias_Freelancer_FreelancerId",
                table: "FreelancerSocialMedias",
                column: "FreelancerId",
                principalTable: "Freelancer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerSpecialization_Freelancer_FreelancersId",
                table: "FreelancerSpecialization",
                column: "FreelancersId",
                principalTable: "Freelancer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentProcess_Freelancer_FreelancerId",
                table: "PaymentProcess",
                column: "FreelancerId",
                principalTable: "Freelancer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectPayment_PaymentMethod_PaymentMethodId",
                table: "ProjectPayment",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Freelancer_FreelancerId",
                table: "Projects",
                column: "FreelancerId",
                principalTable: "Freelancer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Freelancer_FreelancerId",
                table: "Proposals",
                column: "FreelancerId",
                principalTable: "Freelancer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorksOn_Freelancer_FreelancerId",
                table: "WorksOn",
                column: "FreelancerId",
                principalTable: "Freelancer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Freelancer_AspNetUsers_UserId",
                table: "Freelancer");

            migrationBuilder.DropForeignKey(
                name: "FK_Freelancer_Countries_CountryId",
                table: "Freelancer");

            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerLanguagesPairs_Freelancer_FreelancerId",
                table: "FreelancerLanguagesPairs");

            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerSocialMedias_Freelancer_FreelancerId",
                table: "FreelancerSocialMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerSpecialization_Freelancer_FreelancersId",
                table: "FreelancerSpecialization");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentProcess_Freelancer_FreelancerId",
                table: "PaymentProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectPayment_PaymentMethod_PaymentMethodId",
                table: "ProjectPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Freelancer_FreelancerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Freelancer_FreelancerId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_WorksOn_Freelancer_FreelancerId",
                table: "WorksOn");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_ProjectPayment_PaymentMethodId",
                table: "ProjectPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Freelancer",
                table: "Freelancer");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "ProjectPayment");

            migrationBuilder.DropColumn(
                name: "CVFilePath",
                table: "Freelancer");

            migrationBuilder.RenameTable(
                name: "Freelancer",
                newName: "Freelancers");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancer_UserId",
                table: "Freelancers",
                newName: "IX_Freelancers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancer_CountryId",
                table: "Freelancers",
                newName: "IX_Freelancers_CountryId");

            migrationBuilder.AlterColumn<long>(
                name: "SpecializationId",
                table: "Projects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Projects",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FreelancerId",
                table: "Projects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "ProjectPayment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Freelancers",
                table: "Freelancers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerLanguagesPairs_Freelancers_FreelancerId",
                table: "FreelancerLanguagesPairs",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Freelancers_AspNetUsers_UserId",
                table: "Freelancers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Freelancers_Countries_CountryId",
                table: "Freelancers",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerSocialMedias_Freelancers_FreelancerId",
                table: "FreelancerSocialMedias",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerSpecialization_Freelancers_FreelancersId",
                table: "FreelancerSpecialization",
                column: "FreelancersId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentProcess_Freelancers_FreelancerId",
                table: "PaymentProcess",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Freelancers_FreelancerId",
                table: "Projects",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Freelancers_FreelancerId",
                table: "Proposals",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorksOn_Freelancers_FreelancerId",
                table: "WorksOn",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
