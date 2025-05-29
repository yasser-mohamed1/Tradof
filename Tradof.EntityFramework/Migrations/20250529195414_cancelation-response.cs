using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class cancelationresponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CancellationAccepted",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CancellationAcceptedBy",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationAcceptedDate",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationRequestDate",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CancellationRequested",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CancellationRequestedBy",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationResponse",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationAccepted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CancellationAcceptedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CancellationAcceptedDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CancellationRequestDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CancellationRequested",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CancellationRequestedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CancellationResponse",
                table: "Projects");
        }
    }
}
