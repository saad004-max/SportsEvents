using Microsoft.EntityFrameworkCore;
using SportsEventsManagement.Models;

namespace SportsEventsManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // These lines create the tables in your database
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Tournoi> Tournois { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Match> Matchs { get; set; }

        // --- PASTE STARTS HERE ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Safety Rule: Stop SQL Server from complaining about "Loops" when deleting teams
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Equipe1)
                .WithMany()
                .HasForeignKey(m => m.Equipe1Id)
                .OnDelete(DeleteBehavior.Restrict); // If Team 1 is deleted, STOP (don't delete the match automatically)

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Equipe2)
                .WithMany()
                .HasForeignKey(m => m.Equipe2Id)
                .OnDelete(DeleteBehavior.Restrict); // If Team 2 is deleted, STOP
        }
        // --- PASTE ENDS HERE ---
    }
}