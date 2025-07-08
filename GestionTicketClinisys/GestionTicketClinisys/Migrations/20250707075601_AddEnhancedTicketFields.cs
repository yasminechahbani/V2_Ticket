using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicketClinisys.Migrations
{
    /// <inheritdoc />
    public partial class AddEnhancedTicketFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachedFiles",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Tickets",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationDays",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMonths",
                table: "Tickets",
                type: "int",
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

            migrationBuilder.AddColumn<string>(
                name: "RichTextComment",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tickets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachedFiles",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DurationDays",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DurationMonths",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FaultType",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ProviderType",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RichTextComment",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tickets");
        }
    }
}
