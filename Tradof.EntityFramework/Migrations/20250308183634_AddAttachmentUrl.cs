using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectDeliveryTime",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "Attachment",
                table: "PropsalAttachments",
                newName: "AttachmentUrl");

            migrationBuilder.AddColumn<int>(
                name: "Days",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Days",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "AttachmentUrl",
                table: "PropsalAttachments",
                newName: "Attachment");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProjectDeliveryTime",
                table: "Proposals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
