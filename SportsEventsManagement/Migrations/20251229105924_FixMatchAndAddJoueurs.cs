using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsEventsManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixMatchAndAddJoueurs : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_Matchs_Equipe1Id",
                table: "Matchs");

            migrationBuilder.DropIndex(
                name: "IX_Matchs_Equipe2Id",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "Equipe1Id",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "Equipe2Id",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Matchs");

            migrationBuilder.RenameColumn(
                name: "ScoreEquipe2",
                table: "Matchs",
                newName: "ScoreExterieur");

            migrationBuilder.RenameColumn(
                name: "ScoreEquipe1",
                table: "Matchs",
                newName: "ScoreDomicile");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Matchs",
                newName: "DateMatch");

            migrationBuilder.AddColumn<int>(
                name: "EquipeDomicileId",
                table: "Matchs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipeExterieurId",
                table: "Matchs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Joueurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Poste = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Joueurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Joueurs_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_EquipeDomicileId",
                table: "Matchs",
                column: "EquipeDomicileId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_EquipeExterieurId",
                table: "Matchs",
                column: "EquipeExterieurId");

            migrationBuilder.CreateIndex(
                name: "IX_Joueurs_EquipeId",
                table: "Joueurs",
                column: "EquipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Equipes_EquipeDomicileId",
                table: "Matchs",
                column: "EquipeDomicileId",
                principalTable: "Equipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Equipes_EquipeExterieurId",
                table: "Matchs",
                column: "EquipeExterieurId",
                principalTable: "Equipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Equipes_EquipeDomicileId",
                table: "Matchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Equipes_EquipeExterieurId",
                table: "Matchs");

            migrationBuilder.DropTable(
                name: "Joueurs");

            migrationBuilder.DropIndex(
                name: "IX_Matchs_EquipeDomicileId",
                table: "Matchs");

            migrationBuilder.DropIndex(
                name: "IX_Matchs_EquipeExterieurId",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "EquipeDomicileId",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "EquipeExterieurId",
                table: "Matchs");

            migrationBuilder.RenameColumn(
                name: "ScoreExterieur",
                table: "Matchs",
                newName: "ScoreEquipe2");

            migrationBuilder.RenameColumn(
                name: "ScoreDomicile",
                table: "Matchs",
                newName: "ScoreEquipe1");

            migrationBuilder.RenameColumn(
                name: "DateMatch",
                table: "Matchs",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "Equipe1Id",
                table: "Matchs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Equipe2Id",
                table: "Matchs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Matchs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_Equipe1Id",
                table: "Matchs",
                column: "Equipe1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_Equipe2Id",
                table: "Matchs",
                column: "Equipe2Id");

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
    }
}
