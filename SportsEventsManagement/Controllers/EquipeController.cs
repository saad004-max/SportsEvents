using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsEventsManagement.Data;
using SportsEventsManagement.Models;

namespace SportsEventsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquipeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Equipe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipe>>> GetEquipes()
        {
            return await _context.Equipes.ToListAsync();
        }

        // POST: api/Equipe
        [HttpPost]
        public async Task<ActionResult<Equipe>> CreateEquipe(Equipe equipe)
        {
            // 1. Check if a Tournament ID is provided
            if (equipe.TournoiId == null)
            {
                return BadRequest("L'ID du tournoi est requis.");
            }

            // 2. Load the Tournament including its current Teams to check count
            var tournoi = await _context.Tournois
                                        .Include(t => t.Equipes)
                                        .FirstOrDefaultAsync(t => t.Id == equipe.TournoiId);

            if (tournoi == null)
            {
                return NotFound("Tournoi introuvable.");
            }

            // 3. Rule: Cannot join a Draft (Brouillon) or Finished tournament
            if (tournoi.Statut != "Publié")
            {
                return BadRequest($"Impossible d'inscrire une équipe. Le tournoi est '{tournoi.Statut}'.");
            }

            // 4. Rule: Check Available Places
            if (tournoi.Equipes.Count >= tournoi.NombreEquipesMax)
            {
                return BadRequest("Le tournoi est complet. Plus de places disponibles.");
            }

            // 5. Save the Team
            _context.Equipes.Add(equipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEquipes), new { id = equipe.Id }, equipe);
        }

        // DELETE: api/Equipe/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipe(int id)
        {
            // 1. Find the team
            var equipe = await _context.Equipes.FindAsync(id);
            if (equipe == null) return NotFound();

            // 2. Find and delete related matches first!
            // NOTE: If 'EquipeDomicile' is red, check your Match.cs! 
            // It might be named 'Equipe1', 'HomeTeam', or 'Domicile'.
            var matchAssocies = _context.Matchs
                                .Where(m => m.EquipeDomicile.Id == id || m.EquipeExterieur.Id == id);

            if (matchAssocies.Any())
            {
                _context.Matchs.RemoveRange(matchAssocies);
            }

            // 3. Find and delete related players
            // This requires that you have added "public DbSet<Joueur> Joueurs { get; set; }" to AppDbContext
            var joueursAssocies = _context.Joueurs.Where(j => j.EquipeId == id);

            if (joueursAssocies.Any())
            {
                _context.Joueurs.RemoveRange(joueursAssocies);
            }

            // 4. NOW you can safely delete the team
            _context.Equipes.Remove(equipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}