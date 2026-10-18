using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations;

[DbContext(typeof(SystemDBContext))]
[Migration("20261018160000_AssignDriversToVehicles")]
public partial class AssignDriversToVehicles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "VehicleDetailId",
            table: "Drivers",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Drivers_VehicleDetailId",
            table: "Drivers",
            column: "VehicleDetailId");

        migrationBuilder.AddForeignKey(
            name: "FK_Drivers_VehicleDetails_VehicleDetailId",
            table: "Drivers",
            column: "VehicleDetailId",
            principalTable: "VehicleDetails",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Drivers_VehicleDetails_VehicleDetailId",
            table: "Drivers");

        migrationBuilder.DropIndex(
            name: "IX_Drivers_VehicleDetailId",
            table: "Drivers");

        migrationBuilder.DropColumn(
            name: "VehicleDetailId",
            table: "Drivers");
    }
}
