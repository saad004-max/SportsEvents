namespace SportsEventsManagement.Models
{
    public class Joueur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Poste { get; set; } // e.g., Attaquant, Defenseur

        // Foreign Key: Link to the Team
        public int EquipeId { get; set; }
        public Equipe Equipe { get; set; }
    }
}