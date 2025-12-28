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
            return await _context.Tournois.ToListAsync();
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
            if (tournoi == null)
            {
                return NotFound();
            }

            _context.Tournois.Remove(tournoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}