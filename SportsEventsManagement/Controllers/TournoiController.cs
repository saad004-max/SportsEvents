using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsEventsManagement.Data;
using SportsEventsManagement.Models;

namespace SportsEventsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournoiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TournoiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tournoi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournoi>>> GetTournois()
        {
            return await _context.Tournois.Include(t => t.Equipes).ToListAsync();
        }

        // POST: api/Tournoi
        [HttpPost]
        public async Task<ActionResult<Tournoi>> CreateTournoi(Tournoi tournoi)
        {
            _context.Tournois.Add(tournoi);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTournois), new { id = tournoi.Id }, tournoi);
        }

        // DELETE: api/Tournoi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournoi(int id)
        {
            var tournoi = await _context.Tournois.FindAsync(id);
            if (tournoi == null) return NotFound();

            _context.Tournois.Remove(tournoi);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: Publish (Brouillon -> Publié)
        [HttpPatch("{id}/publier")]
        public async Task<IActionResult> PublierTournoi(int id)
        {
            var tournoi = await _context.Tournois.FindAsync(id);
            if (tournoi == null) return NotFound();

            if (tournoi.Statut != "Brouillon")
                return BadRequest("Seul un tournoi 'Brouillon' peut être publié.");

            tournoi.Statut = "Publié";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tournoi publié avec succès.", statut = tournoi.Statut });
        }

        // POST: Generate Matches
        [HttpPost("{id}/generer")]
        public async Task<IActionResult> GenererMatchs(int id)
        {
            var tournoi = await _context.Tournois
                                        .Include(t => t.Equipes)
                                        .Include(t => t.Matchs)
                                        .FirstOrDefaultAsync(t => t.Id == id);

            if (tournoi == null) return NotFound("Tournoi introuvable.");

            if (tournoi.Statut == "Brouillon")
                return BadRequest("Impossible de générer les matchs. Le tournoi est encore en brouillon.");

            if (tournoi.Matchs.Any())
                return BadRequest("Les matchs ont déjà été générés pour ce tournoi.");

            if (tournoi.Equipes.Count < 2)
                return BadRequest("Il faut au moins 2 équipes pour générer des matchs.");

            var equipes = tournoi.Equipes.ToList();
            var matchsGeneres = new List<Match>();
            DateTime dateMatch = tournoi.DateDebut;

            for (int i = 0; i < equipes.Count; i++)
            {
                for (int j = i + 1; j < equipes.Count; j++)
                {
                    // FIX: Using CORRECT names from your new Match.cs
                    var match = new Match
                    {
                        TournoiId = tournoi.Id,
                        EquipeDomicileId = equipes[i].Id,   // FIXED
                        EquipeExterieurId = equipes[j].Id,  // FIXED
                        DateMatch = dateMatch,              // FIXED
                        Lieu = tournoi.Lieu,
                        ScoreDomicile = 0,                  // FIXED
                        ScoreExterieur = 0                  // FIXED
                    };

                    matchsGeneres.Add(match);
                    dateMatch = dateMatch.AddHours(2);
                }
            }

            _context.Matchs.AddRange(matchsGeneres);
            tournoi.Statut = "En Cours";
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{matchsGeneres.Count} matchs générés.", matchs = matchsGeneres });
        }
    }
}