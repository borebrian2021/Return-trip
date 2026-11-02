using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations
{
    /// <inheritdoc />
    public partial class AddCargoBidDepositState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "CargoBids",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepositPaidAt",
                table: "CargoBids",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "CargoBids",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "CargoBids",
                type: "nvarchar(30)",
                nullable: false,
                defaultValue: "NotStarted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "CargoBids");

            migrationBuilder.DropColumn(
                name: "DepositPaidAt",
                table: "CargoBids");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "CargoBids");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "CargoBids");
        }
    }
}
