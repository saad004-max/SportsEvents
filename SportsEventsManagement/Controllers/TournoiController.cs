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

        // GET: api/Tournoi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournoi>> GetTournoi(int id)
        {
            var tournoi = await _context.Tournois
                .Include(t => t.Equipes)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournoi == null)
            {
                return NotFound();
            }

            return tournoi;
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

        // PATCH: Publish
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

        // POST: Generate Bracket (First Round)
        [HttpPost("{id}/generer")]
        public async Task<IActionResult> GenererMatchs(int id)
        {
            var tournoi = await _context.Tournois
                                    .Include(t => t.Equipes)
                                    .Include(t => t.Matchs)
                                    .FirstOrDefaultAsync(t => t.Id == id);

            if (tournoi == null) return NotFound("Tournoi introuvable.");

            // Validations
            if (tournoi.Statut != "Publié") return BadRequest("Le tournoi doit être 'Publié' pour générer le bracket.");
            if (tournoi.Matchs.Any()) return BadRequest("Le bracket existe déjà.");
            if (tournoi.Equipes.Count < 2) return BadRequest("Il faut au moins 2 équipes.");

            // 1. Shuffle Teams
            var random = new Random();
            var equipes = tournoi.Equipes.OrderBy(x => random.Next()).ToList();

            var matchsGeneres = new List<Match>();
            DateTime dateMatch = tournoi.DateDebut;

            // 2. Create Pairs
            for (int i = 0; i < equipes.Count; i += 2)
            {
                if (i + 1 < equipes.Count)
                {
                    var match = new Match
                    {
                        TournoiId = tournoi.Id,
                        EquipeDomicileId = equipes[i].Id,
                        EquipeExterieurId = equipes[i + 1].Id,
                        DateMatch = dateMatch,
                        Lieu = tournoi.Lieu,
                        ScoreDomicile = 0,
                        ScoreExterieur = 0,
                        Tour = 1
                    };
                    matchsGeneres.Add(match);
                    dateMatch = dateMatch.AddHours(2);
                }
            }

            _context.Matchs.AddRange(matchsGeneres);
            tournoi.Statut = "En Cours";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bracket généré (Tour 1)", matchs = matchsGeneres });
        }

        // GET: Teams for a specific tournament
        [HttpGet("{id}/teams")]
        public async Task<ActionResult<IEnumerable<Equipe>>> GetTeamsByTournament(int id)
        {
            var teams = await _context.Equipes
                                      .Where(e => e.TournoiId == id)
                                      .ToListAsync();

            if (teams == null || !teams.Any())
            {
                return Ok(new List<Equipe>());
            }

            return Ok(teams);
        }

        // [FIXED] POST: Generate Next Round
        [HttpPost("{id}/next-round")]
        public async Task<IActionResult> GenerateNextRound(int id)
        {
            var tournoi = await _context.Tournois
                .Include(t => t.Matchs)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournoi == null) return NotFound("Tournoi not found.");

            // 1. Find the current latest round
            if (!tournoi.Matchs.Any()) return BadRequest("No matches found. Generate Round 1 first.");

            int currentRound = tournoi.Matchs.Max(m => m.Tour);
            var currentRoundMatches = tournoi.Matchs
                .Where(m => m.Tour == currentRound)
                .OrderBy(m => m.Id)
                .ToList();

            // 2. Validate: All matches must be finished (No draws allowed!)
            foreach (var match in currentRoundMatches)
            {
                if (match.ScoreDomicile == match.ScoreExterieur)
                {
                    return BadRequest($"Match #{match.Id} is a DRAW. You must update scores to pick a winner before proceeding.");
                }

                // Safety Check: If ID is somehow null, stop
                if (match.EquipeDomicileId == null || match.EquipeExterieurId == null)
                {
                    return BadRequest($"Match #{match.Id} has invalid team data.");
                }
            }

            // 3. Collect Winners
            var winnersIds = new List<int>();
            foreach (var match in currentRoundMatches)
            {
                // [FIX IS HERE] We use (int) to force the conversion because we know they aren't null now
                if (match.ScoreDomicile > match.ScoreExterieur)
                    winnersIds.Add((int)match.EquipeDomicileId);
                else
                    winnersIds.Add((int)match.EquipeExterieurId);
            }

            // 4. Check if Tournament is over
            if (winnersIds.Count < 2)
            {
                return BadRequest("Tournament is finished! We have a Champion.");
            }

            // 5. Create Next Round Matches
            var nextRoundMatches = new List<Match>();
            DateTime nextDate = currentRoundMatches.Max(m => m.DateMatch).AddDays(2);

            for (int i = 0; i < winnersIds.Count; i += 2)
            {
                if (i + 1 < winnersIds.Count)
                {
                    var newMatch = new Match
                    {
                        TournoiId = tournoi.Id,
                        EquipeDomicileId = winnersIds[i],
                        EquipeExterieurId = winnersIds[i + 1],
                        DateMatch = nextDate,
                        Lieu = tournoi.Lieu,
                        Tour = currentRound + 1, // Increment Round Number
                        ScoreDomicile = 0,
                        ScoreExterieur = 0
                    };
                    nextRoundMatches.Add(newMatch);
                    nextDate = nextDate.AddHours(4);
                }
            }

            _context.Matchs.AddRange(nextRoundMatches);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Round {currentRound + 1} generated successfully!", matches = nextRoundMatches });
        }
    }
}