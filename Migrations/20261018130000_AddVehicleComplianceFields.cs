using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations;

[DbContext(typeof(SystemDBContext))]
[Migration("20261018130000_AddVehicleComplianceFields")]
public partial class AddVehicleComplianceFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "Capacity",
            table: "VehicleDetails",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "InspectionExpiryDate",
            table: "VehicleDetails",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InspectionReportType",
            table: "VehicleDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InspectionReportUrl",
            table: "VehicleDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "InsuranceExpiryDate",
            table: "VehicleDetails",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InsuranceDocumentType",
            table: "VehicleDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InsuranceDocumentUrl",
            table: "VehicleDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "LastCargoTransported",
            table: "VehicleDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Capacity", table: "VehicleDetails");
        migrationBuilder.DropColumn(name: "InspectionExpiryDate", table: "VehicleDetails");
        migrationBuilder.DropColumn(name: "InspectionReportType", table: "VehicleDetails");
        migrationBuilder.DropColumn(name: "InspectionReportUrl", table: "VehicleDetails");
        migrationBuilder.DropColumn(name: "InsuranceExpiryDate", table: "VehicleDetails");
        migrationBuilder.DropColumn(name: "InsuranceDocumentType", table: "VehicleDetails");
        migrationBuilder.DropColumn(name: "InsuranceDocumentUrl", table: "VehicleDetails");
        migrationBuilder.DropColumn(name: "LastCargoTransported", table: "VehicleDetails");
    }
}
