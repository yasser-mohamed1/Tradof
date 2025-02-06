using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tradof.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class updates_in_types_of_Urls_and_DeliveryTime : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "NewProjectDeliveryTime",
				table: "Proposals",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)); 

			migrationBuilder.Sql(@"
        UPDATE Proposals
        SET NewProjectDeliveryTime = DATEADD(SECOND, ProjectDeliveryTime, '1970-01-01')
    ");

			migrationBuilder.DropColumn(
				name: "ProjectDeliveryTime",
				table: "Proposals");

			migrationBuilder.RenameColumn(
				table: "Proposals",
				name: "NewProjectDeliveryTime",
				newName: "ProjectDeliveryTime");

			migrationBuilder.AlterColumn<long>(
				name: "FileSize",
				table: "Files",
				type: "bigint",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int");
		}
		/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<int>(
        name: "OldProjectDeliveryTime",
        table: "Proposals",
        type: "int",
        nullable: false,
        defaultValue: 0);

    migrationBuilder.Sql(@"
        UPDATE Proposals
        SET OldProjectDeliveryTime = DATEDIFF(SECOND, '1970-01-01', ProjectDeliveryTime)
    ");

    // Drop the new column
    migrationBuilder.DropColumn(
        name: "ProjectDeliveryTime",
        table: "Proposals");

    migrationBuilder.RenameColumn(
        table: "Proposals",
        name: "OldProjectDeliveryTime",
        newName: "ProjectDeliveryTime");

    migrationBuilder.AlterColumn<int>(
        name: "FileSize",
        table: "Files",
        type: "int",
        nullable: false,
        oldClrType: typeof(long),
        oldType: "bigint");
}
    }
}
