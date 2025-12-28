using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsEventsManagement.Models
{
    public class Match
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string Lieu { get; set; } = string.Empty;
        public int ScoreEquipe1 { get; set; }
        public int ScoreEquipe2 { get; set; }
        public string Statut { get; set; } = "Prevus"; // Prevus, EnCours, Termine

        // Foreign Keys (The IDs)
        public int TournoiId { get; set; }
        public int Equipe1Id { get; set; }
        public int Equipe2Id { get; set; }

        // Navigation Properties (The Links - This is what was missing!)
        [ForeignKey("TournoiId")]
        public virtual Tournoi? Tournoi { get; set; }

        [ForeignKey("Equipe1Id")]
        public virtual Equipe? Equipe1 { get; set; }

        [ForeignKey("Equipe2Id")]
        public virtual Equipe? Equipe2 { get; set; }
    }
}