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

        // GET: api/Match
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
                    homeTeam = m.EquipeDomicile != null ? m.EquipeDomicile.Nom : "Unknown",
                    awayTeam = m.EquipeExterieur != null ? m.EquipeExterieur.Nom : "Unknown",
                    tournoiName = m.Tournoi != null ? m.Tournoi.Nom : "Unknown Tournament"
                })
                .ToListAsync();

            return Ok(matches);
        }

        // GET: api/Match/tournoi/5
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
        public async Task<IActionResult> GetMatch(int id)
        {
            var match = await _context.Matchs
                .Include(m => m.EquipeDomicile)
                .Include(m => m.EquipeExterieur)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null) return NotFound();

            return Ok(new
            {
                id = match.Id,
                homeTeam = match.EquipeDomicile?.Nom ?? "Unknown",
                awayTeam = match.EquipeExterieur?.Nom ?? "Unknown",
                scoreDomicile = match.ScoreDomicile,
                scoreExterieur = match.ScoreExterieur
            });
        }

        // PUT: api/Match/5/score
        [HttpPut("{id}/score")]
        public async Task<IActionResult> UpdateScore(int id, [FromBody] ScoreUpdateDto scoreDto)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null) return NotFound();

            match.ScoreDomicile = scoreDto.ScoreDomicile;
            match.ScoreExterieur = scoreDto.ScoreExterieur;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Score updated" });
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

        // --- DTO CLASS IS NOW INSIDE THE CONTROLLER TO PREVENT ERRORS ---
        public class ScoreUpdateDto
        {
            public int ScoreDomicile { get; set; }
            public int ScoreExterieur { get; set; }
        }
    }
}