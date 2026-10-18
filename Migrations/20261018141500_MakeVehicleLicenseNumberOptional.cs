using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations;

[DbContext(typeof(SystemDBContext))]
[Migration("20261018141500_MakeVehicleLicenseNumberOptional")]
public partial class MakeVehicleLicenseNumberOptional : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "LicenseNumber",
            table: "VehicleDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE VehicleDetails SET LicenseNumber = N'' WHERE LicenseNumber IS NULL");

        migrationBuilder.AlterColumn<string>(
            name: "LicenseNumber",
            table: "VehicleDetails",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
