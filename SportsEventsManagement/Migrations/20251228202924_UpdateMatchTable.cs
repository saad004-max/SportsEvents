using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsEventsManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatchTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Equipes_Equipe1Id",
                table: "Matchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Equipes_Equipe2Id",
                table: "Matchs");

            migrationBuilder.RenameColumn(
                name: "DateHeure",
                table: "Matchs",
                newName: "Date");

            migrationBuilder.AlterColumn<int>(
                name: "Equipe2Id",
                table: "Matchs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Equipe1Id",
                table: "Matchs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreEquipe1",
                table: "Matchs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScoreEquipe2",
                table: "Matchs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Equipes_Equipe1Id",
                table: "Matchs",
                column: "Equipe1Id",
                principalTable: "Equipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Equipes_Equipe2Id",
                table: "Matchs",
                column: "Equipe2Id",
                principalTable: "Equipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Equipes_Equipe1Id",
                table: "Matchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Equipes_Equipe2Id",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "ScoreEquipe1",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "ScoreEquipe2",
                table: "Matchs");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Matchs",
                newName: "DateHeure");

            migrationBuilder.AlterColumn<int>(
                name: "Equipe2Id",
                table: "Matchs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Equipe1Id",
                table: "Matchs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Equipes_Equipe1Id",
                table: "Matchs",
                column: "Equipe1Id",
                principalTable: "Equipes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Equipes_Equipe2Id",
                table: "Matchs",
                column: "Equipe2Id",
                principalTable: "Equipes",
                principalColumn: "Id");
        }
    }
}
