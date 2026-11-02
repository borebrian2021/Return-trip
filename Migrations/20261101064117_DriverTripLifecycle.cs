using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations
{
    /// <inheritdoc />
    public partial class DriverTripLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CargoBids_DriverId",
                table: "CargoBids");

            migrationBuilder.AddColumn<string>(
                name: "LastCargoTransported",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDeliveryAt",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastTripReference",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TripAssignedAt",
                table: "CargoBids",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TripStatus",
                table: "CargoBids",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "AwaitingDeposit");

            migrationBuilder.Sql(
                """
                UPDATE [CargoBids]
                SET [TripStatus] = CASE
                        WHEN [PaymentStatus] = 'Paid' AND [DepositPaidAt] IS NOT NULL THEN 'ReadyForPickup'
                        ELSE 'AwaitingDeposit'
                    END,
                    [TripAssignedAt] = CASE
                        WHEN [DriverId] IS NOT NULL THEN COALESCE([AcceptedAt], [CreatedAt])
                        ELSE NULL
                    END
                WHERE [Status] = 'Accepted';
                """);

            migrationBuilder.CreateTable(
                name: "CargoPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoBidId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubmittedByUserAccountId = table.Column<int>(type: "int", nullable: false),
                    VerifiedByCompanyAgentId = table.Column<int>(type: "int", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationComments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoPayments_CargoBids_CargoBidId",
                        column: x => x.CargoBidId,
                        principalTable: "CargoBids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelAdvances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoBidId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvidenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReconciledByCompanyAgentId = table.Column<int>(type: "int", nullable: true),
                    ReconciledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelAdvances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelAdvances_CargoBids_CargoBidId",
                        column: x => x.CargoBidId,
                        principalTable: "CargoBids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoBidId = table.Column<int>(type: "int", nullable: false),
                    UserAccountId = table.Column<int>(type: "int", nullable: true),
                    Activity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripActivities_CargoBids_CargoBidId",
                        column: x => x.CargoBidId,
                        principalTable: "CargoBids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_DriverId_TripStatus",
                table: "CargoBids",
                columns: new[] { "DriverId", "TripStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_CargoPayments_CargoBidId",
                table: "CargoPayments",
                column: "CargoBidId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoPayments_Reference",
                table: "CargoPayments",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelAdvances_CargoBidId",
                table: "FuelAdvances",
                column: "CargoBidId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelAdvances_Reference",
                table: "FuelAdvances",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripActivities_CargoBidId",
                table: "TripActivities",
                column: "CargoBidId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoPayments");

            migrationBuilder.DropTable(
                name: "FuelAdvances");

            migrationBuilder.DropTable(
                name: "TripActivities");

            migrationBuilder.DropIndex(
                name: "IX_CargoBids_DriverId_TripStatus",
                table: "CargoBids");

            migrationBuilder.DropColumn(
                name: "LastCargoTransported",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LastDeliveryAt",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LastTripReference",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "TripAssignedAt",
                table: "CargoBids");

            migrationBuilder.DropColumn(
                name: "TripStatus",
                table: "CargoBids");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_DriverId",
                table: "CargoBids",
                column: "DriverId");
        }
    }
}
