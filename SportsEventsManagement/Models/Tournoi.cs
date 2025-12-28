using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
        public string Statut { get; set; } = "Brouillon";

        // Lists to hold related data
        public List<Equipe> Equipes { get; set; } = new List<Equipe>();
        public List<Match> Matchs { get; set; } = new List<Match>();
    }
}