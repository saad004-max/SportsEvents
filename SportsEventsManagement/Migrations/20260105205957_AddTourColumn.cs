using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsEventsManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddTourColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tour",
                table: "Matchs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tour",
                table: "Matchs");
        }
    }
}
