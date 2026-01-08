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

        // POST: api/Equipe (Used by AddTeam.razor)
        [HttpPost]
        public async Task<ActionResult<Equipe>> CreateEquipe(EquipeDto dto)
        {
            // 1. Find Tournament
            var tournoi = await _context.Tournois.Include(t => t.Equipes).FirstOrDefaultAsync(t => t.Id == dto.TournoiId);
            if (tournoi == null) return NotFound("Tournament not found");

            // 2. Validate: Cannot add if tournament is full or already started
            if (tournoi.Statut != "Brouillon") return BadRequest("Cannot add teams to a started tournament.");
            if (tournoi.Equipes.Count >= tournoi.NombreEquipesMax) return BadRequest("Tournament is full.");

            // 3. Save Team
            var equipe = new Equipe
            {
                Nom = dto.Nom,
                Logo = dto.Logo,
                TournoiId = dto.TournoiId
            };

            _context.Equipes.Add(equipe);
            await _context.SaveChangesAsync();

            return Ok(equipe);
        }

        // [NEW] DELETE: api/Equipe/5 (Used by TournamentDetails)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipe(int id)
        {
            var equipe = await _context.Equipes.FindAsync(id);
            if (equipe == null) return NotFound();

            // Check if matches started
            var hasMatches = await _context.Matchs.AnyAsync(m => m.EquipeDomicileId == id || m.EquipeExterieurId == id);
            if (hasMatches)
            {
                return BadRequest("Cannot delete this team because they are already in a match.");
            }

            _context.Equipes.Remove(equipe);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class EquipeDto
        {
            public string Nom { get; set; } = "";
            public string Logo { get; set; } = "";
            public int TournoiId { get; set; }
        }
    }
}