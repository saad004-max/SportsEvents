using Microsoft.EntityFrameworkCore;
using SportsEventsManagement.Models;

namespace SportsEventsManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- ALL TABLES ---
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Tournoi> Tournois { get; set; }
        public DbSet<Match> Matchs { get; set; }
        public DbSet<Joueur> Joueurs { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; } // <--- This was missing!

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Match - Home Team relationship
            modelBuilder.Entity<Match>()
                .HasOne(m => m.EquipeDomicile)
                .WithMany()
                .HasForeignKey(m => m.EquipeDomicileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Match - Away Team relationship
            modelBuilder.Entity<Match>()
                .HasOne(m => m.EquipeExterieur)
                .WithMany()
                .HasForeignKey(m => m.EquipeExterieurId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}