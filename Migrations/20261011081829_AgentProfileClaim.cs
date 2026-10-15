using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations
{
    /// <inheritdoc />
    public partial class AgentProfileClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyAgents_AspNetUsers_UserAccountId",
                table: "CompanyAgents");

            migrationBuilder.AlterColumn<int>(
                name: "UserAccountId",
                table: "CompanyAgents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyAgents_AspNetUsers_UserAccountId",
                table: "CompanyAgents",
                column: "UserAccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyAgents_AspNetUsers_UserAccountId",
                table: "CompanyAgents");

            migrationBuilder.AlterColumn<int>(
                name: "UserAccountId",
                table: "CompanyAgents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyAgents_AspNetUsers_UserAccountId",
                table: "CompanyAgents",
                column: "UserAccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
