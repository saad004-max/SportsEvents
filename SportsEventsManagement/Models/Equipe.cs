using System.Text.Json.Serialization;

namespace SportsEventsManagement.Models
{
    public class Equipe
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Couleur { get; set; } = string.Empty;

        // Foreign Key to Tournoi
        public int? TournoiId { get; set; }
        [JsonIgnore]
        public Tournoi? Tournoi { get; set; }
    }
}