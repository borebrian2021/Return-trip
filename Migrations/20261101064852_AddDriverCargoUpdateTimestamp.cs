using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverCargoUpdateTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastCargoUpdatedAt",
                table: "Drivers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCargoUpdatedAt",
                table: "Drivers");
        }
    }
}
