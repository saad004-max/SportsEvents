using SportsEventsManagement.Data;
using SportsEventsManagement.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace SportsEventsManagement.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Utilisateur?> Register(Utilisateur utilisateur)
        {
            // Check if email already exists
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == utilisateur.Email))
            {
                return null; // Email already taken
            }

            // Hash the password securely
            utilisateur.MotDePasse = BCrypt.Net.BCrypt.HashPassword(utilisateur.MotDePasse);

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
            return utilisateur;
        }

        public async Task<Utilisateur?> Login(string email, string password)
        {
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.MotDePasse))
            {
                return null; // Wrong email or password
            }

            return user; // Success
        }
    }
}