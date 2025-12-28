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
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            // Include details about the teams and tournament so you see names, not just IDs
            return await _context.Matchs
                .Include(m => m.Equipe1)
                .Include(m => m.Equipe2)
                .Include(m => m.Tournoi)
                .ToListAsync();
        }

        // POST: api/Match
        [HttpPost]
        public async Task<ActionResult<Match>> CreateMatch(Match match)
        {
            _context.Matchs.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatches), new { id = match.Id }, match);
        }

        // PUT: api/Match/5 (To update scores later)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(int id, Match match)
        {
            if (id != match.Id) return BadRequest();

            _context.Entry(match).State = EntityState.Modified;
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
    
    // PATCH: api/Match/5/score
        // This specific endpoint updates ONLY the score and status
        [HttpPatch("{id}/score")]
        public async Task<IActionResult> UpdateScore(int id, int score1, int score2)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null) return NotFound();

            // Update the data
            match.ScoreEquipe1 = score1;
            match.ScoreEquipe2 = score2;
            match.Statut = "Termine"; // Mark match as Finished

            await _context.SaveChangesAsync();
            return Ok(match);
        } }
    }