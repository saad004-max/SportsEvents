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

        // [NEW] GET: api/Tournoi/5
        // This is required for the TournamentDetails page to work!
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournoi>> GetTournoi(int id)
        {
            var tournoi = await _context.Tournois
                .Include(t => t.Equipes) // Important: Load the teams too!
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

        // POST: Generate Bracket (Round 1)
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

            // 1. Shuffle Teams (Random Order)
            var random = new Random();
            var equipes = tournoi.Equipes.OrderBy(x => random.Next()).ToList();

            var matchsGeneres = new List<Match>();
            DateTime dateMatch = tournoi.DateDebut;

            // 2. Create Pairs (1 vs 2, 3 vs 4, etc.)
            for (int i = 0; i < equipes.Count; i += 2)
            {
                // Ensure we have a pair (handle odd numbers by skipping the last one for now)
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
                        Tour = 1 // This marks it as "Round 1"
                    };
                    matchsGeneres.Add(match);
                    dateMatch = dateMatch.AddHours(2); // Stagger matches by 2 hours
                }
            }

            _context.Matchs.AddRange(matchsGeneres);
            tournoi.Statut = "En Cours";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bracket généré (Tour 1)", matchs = matchsGeneres });
        }
        // GET: api/Tournoi/5/teams
        [HttpGet("{id}/teams")]
        public async Task<ActionResult<IEnumerable<Equipe>>> GetTeamsByTournament(int id)
        {
            // OPTION A: If you have a direct relationship (Equipe has a TournoiId)
            /*
            var teams = await _context.Equipes
                .Where(e => e.TournoiId == id)
                .ToListAsync();
            */

            // OPTION B: If you use a Join Table (TournoiEquipe) - MORE COMMON
            // This assumes you have a Many-to-Many relationship
            /*
            var teams = await _context.TournoiEquipes
                .Where(te => te.TournoiId == id)
                .Select(te => te.Equipe)
                .ToListAsync();
            */

            // OPTION C: If you don't have relationships set up yet and just want all teams (Temporary)
            var teams = await _context.Equipes.ToListAsync();

            if (teams == null)
            {
                return NotFound();
            }

            return Ok(teams);
        }
    }
}