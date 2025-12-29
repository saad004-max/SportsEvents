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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatchs()
        {
            return await _context.Matchs
                .Include(m => m.EquipeDomicile)
                .Include(m => m.EquipeExterieur)
                .ToListAsync();
        }

        [HttpPut("{id}/score")]
        public async Task<IActionResult> UpdateScore(int id, [FromBody] ScoreUpdateDto scoreDto)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null) return NotFound();

            match.ScoreDomicile = scoreDto.ScoreDomicile;
            match.ScoreExterieur = scoreDto.ScoreExterieur;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Score mis à jour", match });
        }
    }

    public class ScoreUpdateDto
    {
        public int ScoreDomicile { get; set; }
        public int ScoreExterieur { get; set; }
    }
}