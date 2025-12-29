using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsEventsManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddTournoiMaxLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NombreEquipesMax",
                table: "Tournois",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreEquipesMax",
                table: "Tournois");
        }
    }
}
