using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations;

[DbContext(typeof(SystemDBContext))]
[Migration("20261018170000_AddCompanyAgentIdNumber")]
public partial class AddCompanyAgentIdNumber : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "IdNumber",
            table: "CompanyAgents",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_CompanyAgents_IdNumber",
            table: "CompanyAgents",
            column: "IdNumber",
            unique: true,
            filter: "[IdNumber] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_CompanyAgents_IdNumber",
            table: "CompanyAgents");

        migrationBuilder.DropColumn(
            name: "IdNumber",
            table: "CompanyAgents");
    }
}
