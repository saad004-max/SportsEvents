using System.ComponentModel.DataAnnotations.Schema;

namespace SportsEventsManagement.Models
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime DateMatch { get; set; }
        public string Lieu { get; set; } = string.Empty;

        // [NEW] This property tracks the Round (1 = Round of 16, 2 = Quarter-final, etc.)
        public int Tour { get; set; } = 1;

        // Home Team
        public int? EquipeDomicileId { get; set; }
        [ForeignKey("EquipeDomicileId")]
        public Equipe EquipeDomicile { get; set; }

        // Away Team
        public int? EquipeExterieurId { get; set; }
        [ForeignKey("EquipeExterieurId")]
        public Equipe EquipeExterieur { get; set; }

        // Link to Tournament
        public int TournoiId { get; set; }
        public Tournoi Tournoi { get; set; }

        // Scores
        public int ScoreDomicile { get; set; }
        public int ScoreExterieur { get; set; }
    }
}