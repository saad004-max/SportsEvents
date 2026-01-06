using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsEventsManagement.Data;
using SportsEventsManagement.Models;

namespace SportsEventsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatchController(AppDbContext context)
        {
            _context = context;
        }

        // [CHANGED] GET: api/Match (List of all matches)
        // Now returns FLATTENED data to prevent errors and missing names
        [HttpGet]
        public async Task<IActionResult> GetMatchs()
        {
            var matches = await _context.Matchs
                .OrderByDescending(m => m.DateMatch)
                .Select(m => new
                {
                    id = m.Id,
                    dateMatch = m.DateMatch,
                    lieu = m.Lieu,
                    tour = m.Tour,
                    scoreDomicile = m.ScoreDomicile,
                    scoreExterieur = m.ScoreExterieur,

                    // SAFE STRINGS
                    homeTeam = m.EquipeDomicile != null ? m.EquipeDomicile.Nom : "Unknown",
                    awayTeam = m.EquipeExterieur != null ? m.EquipeExterieur.Nom : "Unknown",
                    tournoiName = m.Tournoi != null ? m.Tournoi.Nom : "Unknown Tournament"
                })
                .ToListAsync();

            return Ok(matches);
        }

        // GET: api/Match/tournoi/5 (For the Bracket)
        [HttpGet("tournoi/{tournoiId}")]
        public async Task<IActionResult> GetMatchesByTournoi(int tournoiId)
        {
            var matches = await _context.Matchs
                .Where(m => m.TournoiId == tournoiId)
                .OrderBy(m => m.DateMatch)
                .Select(m => new
                {
                    id = m.Id,
                    dateMatch = m.DateMatch,
                    tour = m.Tour,
                    homeScore = m.ScoreDomicile,
                    awayScore = m.ScoreExterieur,
                    homeTeam = m.EquipeDomicile != null ? m.EquipeDomicile.Nom : "Unknown",
                    awayTeam = m.EquipeExterieur != null ? m.EquipeExterieur.Nom : "Unknown"
                })
                .ToListAsync();

            return Ok(matches);
        }

        // GET: api/Match/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Match>> GetMatch(int id)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null) return NotFound();
            return match;
        }

        // PUT: api/Match/5/score
        [HttpPut("{id}/score")]
        public async Task<IActionResult> UpdateScore(int id, int scoreDom, int scoreExt)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null) return NotFound();

            match.ScoreDomicile = scoreDom;
            match.ScoreExterieur = scoreExt;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Match/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null) return NotFound();
            _context.Matchs.Remove(match);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}