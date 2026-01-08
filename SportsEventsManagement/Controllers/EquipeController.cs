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

            // 2. Load the Tournament
            var tournoi = await _context.Tournois
                                        .Include(t => t.Equipes)
                                        .FirstOrDefaultAsync(t => t.Id == equipe.TournoiId);

            if (tournoi == null)
            {
                return NotFound("Tournoi introuvable.");
            }

            // [CHANGE] 3. Rule: Check Status
            // We now ALLOW "Brouillon" and "Publié". We only block if it's already started or finished.
            if (tournoi.Statut == "En Cours" || tournoi.Statut == "Terminé")
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
            var equipe = await _context.Equipes.FindAsync(id);
            if (equipe == null) return NotFound();

            var matchAssocies = _context.Matchs
                                .Where(m => m.EquipeDomicile.Id == id || m.EquipeExterieur.Id == id);

            if (matchAssocies.Any())
            {
                _context.Matchs.RemoveRange(matchAssocies);
            }

            var joueursAssocies = _context.Joueurs.Where(j => j.EquipeId == id);
            if (joueursAssocies.Any())
            {
                _context.Joueurs.RemoveRange(joueursAssocies);
            }

            _context.Equipes.Remove(equipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/Tournoi/5/teams
        [HttpGet("{id}/teams")]
        public async Task<ActionResult<IEnumerable<Equipe>>> GetTeamsByTournament(int id)
        {
            // This explicitly filters teams that belong to the Tournament ID provided in the URL
            var teams = await _context.Equipes
                                      .Where(e => e.TournoiId == id)
                                      .ToListAsync();

            return Ok(teams);
        }
    }
}