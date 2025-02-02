using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanySocialMedia_Companies_CompanyId",
                table: "CompanySocialMedia");

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
                name: "FK_PaymentMethod_Freelancer_FreelancerId",
                table: "PaymentMethod");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentMethod",
                table: "PaymentMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Freelancer",
                table: "Freelancer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanySocialMedia",
                table: "CompanySocialMedia");

            migrationBuilder.RenameTable(
                name: "PaymentMethod",
                newName: "PaymentMethods");

            migrationBuilder.RenameTable(
                name: "Freelancer",
                newName: "Freelancers");

            migrationBuilder.RenameTable(
                name: "CompanySocialMedia",
                newName: "CompanySocialMedias");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethod_FreelancerId",
                table: "PaymentMethods",
                newName: "IX_PaymentMethods_FreelancerId");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancer_UserId",
                table: "Freelancers",
                newName: "IX_Freelancers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancer_CountryId",
                table: "Freelancers",
                newName: "IX_Freelancers_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanySocialMedia_CompanyId",
                table: "CompanySocialMedias",
                newName: "IX_CompanySocialMedias_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Freelancers",
                table: "Freelancers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanySocialMedias",
                table: "CompanySocialMedias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanySocialMedias_Companies_CompanyId",
                table: "CompanySocialMedias",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_PaymentMethods_Freelancers_FreelancerId",
                table: "PaymentMethods",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentProcess_Freelancers_FreelancerId",
                table: "PaymentProcess",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectPayment_PaymentMethods_PaymentMethodId",
                table: "ProjectPayment",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanySocialMedias_Companies_CompanyId",
                table: "CompanySocialMedias");

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
                name: "FK_PaymentMethods_Freelancers_FreelancerId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentProcess_Freelancers_FreelancerId",
                table: "PaymentProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectPayment_PaymentMethods_PaymentMethodId",
                table: "ProjectPayment");

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
                name: "PK_PaymentMethods",
                table: "PaymentMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Freelancers",
                table: "Freelancers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanySocialMedias",
                table: "CompanySocialMedias");

            migrationBuilder.RenameTable(
                name: "PaymentMethods",
                newName: "PaymentMethod");

            migrationBuilder.RenameTable(
                name: "Freelancers",
                newName: "Freelancer");

            migrationBuilder.RenameTable(
                name: "CompanySocialMedias",
                newName: "CompanySocialMedia");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethods_FreelancerId",
                table: "PaymentMethod",
                newName: "IX_PaymentMethod_FreelancerId");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancers_UserId",
                table: "Freelancer",
                newName: "IX_Freelancer_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Freelancers_CountryId",
                table: "Freelancer",
                newName: "IX_Freelancer_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanySocialMedias_CompanyId",
                table: "CompanySocialMedia",
                newName: "IX_CompanySocialMedia_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethod",
                table: "PaymentMethod",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Freelancer",
                table: "Freelancer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanySocialMedia",
                table: "CompanySocialMedia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanySocialMedia_Companies_CompanyId",
                table: "CompanySocialMedia",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_PaymentMethod_Freelancer_FreelancerId",
                table: "PaymentMethod",
                column: "FreelancerId",
                principalTable: "Freelancer",
                principalColumn: "Id");

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
    }
}
