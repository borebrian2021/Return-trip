using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations
{
    /// <inheritdoc />
    public partial class Addedcompaniesfixsss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_CompanyAgents_VehicleDetailId'
      AND object_id = OBJECT_ID(N'[dbo].[CompanyAgents]'))
    DROP INDEX [IX_CompanyAgents_VehicleDetailId] ON [dbo].[CompanyAgents];

IF COL_LENGTH(N'dbo.CompanyAgents', N'VehicleDetailId') IS NOT NULL
    ALTER TABLE [dbo].[CompanyAgents] DROP COLUMN [VehicleDetailId];");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "VehicleDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql(@"
IF EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'IX_Drivers_VehicleDetailId'
            AND object_id = OBJECT_ID(N'[dbo].[Drivers]'))
        DROP INDEX [IX_Drivers_VehicleDetailId] ON [dbo].[Drivers];");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "VehicleDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "VehicleDetailId",
                table: "CompanyAgents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAgents_VehicleDetailId",
                table: "CompanyAgents",
                column: "VehicleDetailId");
        }
    }
}
