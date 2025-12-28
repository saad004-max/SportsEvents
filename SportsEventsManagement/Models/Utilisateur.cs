using System.ComponentModel.DataAnnotations;

namespace SportsEventsManagement.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Role { get; set; } = "Spectateur"; // Admin, Arbitre, Organisateur
    }
}