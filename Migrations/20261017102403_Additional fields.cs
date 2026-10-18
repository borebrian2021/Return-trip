using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations
{
    /// <inheritdoc />
    public partial class Additionalfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DistanceKm",
                table: "Cargoes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PickupDateTime",
                table: "Cargoes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PickupDeadlineAt",
                table: "Cargoes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverEmail",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverPhone",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderContactName",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderPhone",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceKm",
                table: "Cargoes");

            migrationBuilder.DropColumn(
                name: "PickupDateTime",
                table: "Cargoes");

            migrationBuilder.DropColumn(
                name: "PickupDeadlineAt",
                table: "Cargoes");

            migrationBuilder.DropColumn(
                name: "ReceiverEmail",
                table: "Cargoes");

            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Cargoes");

            migrationBuilder.DropColumn(
                name: "ReceiverPhone",
                table: "Cargoes");

            migrationBuilder.DropColumn(
                name: "SenderContactName",
                table: "Cargoes");

            migrationBuilder.DropColumn(
                name: "SenderPhone",
                table: "Cargoes");
        }
    }
}
