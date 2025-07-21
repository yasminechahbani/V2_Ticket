using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicketClinisys.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnwantedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FaultType",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ProviderType",
                table: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Tickets",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FaultType",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderType",
                table: "Tickets",
                type: "int",
                nullable: true);
        }
    }
}
