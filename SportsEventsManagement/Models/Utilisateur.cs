using System.ComponentModel.DataAnnotations;

namespace SportsEventsManagement.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }

        // Fields required by your errors
        public string Nom { get; set; } = string.Empty;       // Fixes 'Nom' error
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty; // Fixes 'Telephone' error

        // Handling the Password confusion
        // Your Service likely uses 'MotDePasse', your Controller uses 'Password'
        // Let's keep 'MotDePasse' as the main one to satisfy the compiler errors.
        public string MotDePasse { get; set; } = string.Empty;

        // New field for the roles
        public string Role { get; set; } = "User";
    }
}