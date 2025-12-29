using System.Text.Json.Serialization;

namespace SportsEventsManagement.Models
{
    public class Tournoi
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Lieu { get; set; } = string.Empty;
        public string Sport { get; set; } = string.Empty;

        // Default status is "Brouillon" as per your design
        public string Statut { get; set; } = "Brouillon";

        // [NEW] Limit the number of teams (e.g., 8, 16, 32)
        public int NombreEquipesMax { get; set; } = 16;

        // Lists
        public List<Equipe> Equipes { get; set; } = new List<Equipe>();
        public List<Match> Matchs { get; set; } = new List<Match>();
    }
}