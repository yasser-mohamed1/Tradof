using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class editProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FreelancerId",
                table: "ProposalEditRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "ProposalEditRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AcceptedProposalId",
                table: "Projects",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProposalEditRequests_FreelancerId",
                table: "ProposalEditRequests",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalEditRequests_ProjectId",
                table: "ProposalEditRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AcceptedProposalId",
                table: "Projects",
                column: "AcceptedProposalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Proposals_AcceptedProposalId",
                table: "Projects",
                column: "AcceptedProposalId",
                principalTable: "Proposals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalEditRequests_Freelancers_FreelancerId",
                table: "ProposalEditRequests",
                column: "FreelancerId",
                principalTable: "Freelancers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalEditRequests_Projects_ProjectId",
                table: "ProposalEditRequests",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Proposals_AcceptedProposalId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalEditRequests_Freelancers_FreelancerId",
                table: "ProposalEditRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalEditRequests_Projects_ProjectId",
                table: "ProposalEditRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProposalEditRequests_FreelancerId",
                table: "ProposalEditRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProposalEditRequests_ProjectId",
                table: "ProposalEditRequests");

            migrationBuilder.DropIndex(
                name: "IX_Projects_AcceptedProposalId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FreelancerId",
                table: "ProposalEditRequests");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProposalEditRequests");

            migrationBuilder.DropColumn(
                name: "AcceptedProposalId",
                table: "Projects");
        }
    }
}
