using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsEventsManagement.Data;
using SportsEventsManagement.Models;

namespace SportsEventsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JoueurController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JoueurController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Joueur
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Joueur>>> GetJoueurs()
        {
            return await _context.Joueurs.ToListAsync();
        }

        // GET: api/Joueur/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Joueur>> GetJoueur(int id)
        {
            var joueur = await _context.Joueurs.FindAsync(id);

            if (joueur == null)
            {
                return NotFound();
            }

            return joueur;
        }

        // POST: api/Joueur
        [HttpPost]
        public async Task<ActionResult<Joueur>> PostJoueur(Joueur joueur)
        {
            // Optional: Verify that the Team exists before adding the player
            var equipe = await _context.Equipes.FindAsync(joueur.EquipeId);
            if (equipe == null)
            {
                return BadRequest("L'équipe spécifiée n'existe pas.");
            }

            _context.Joueurs.Add(joueur);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJoueur", new { id = joueur.Id }, joueur);
        }

        // DELETE: api/Joueur/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJoueur(int id)
        {
            var joueur = await _context.Joueurs.FindAsync(id);
            if (joueur == null)
            {
                return NotFound();
            }

            _context.Joueurs.Remove(joueur);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}