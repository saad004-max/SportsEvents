using SportsEventsManagement.Data;
using SportsEventsManagement.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.Extensions.Configuration; // [NEW] Needed for reading appsettings
using Microsoft.IdentityModel.Tokens;     // [NEW] Needed for security keys
using System.IdentityModel.Tokens.Jwt;    // [NEW] Needed for creating tokens
using System.Security.Claims;             // [NEW] Needed for adding user info to token
using System.Text;                        // [NEW] Needed for Encoding

namespace SportsEventsManagement.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; // [NEW] Add this field

        // [CHANGE] Inject IConfiguration here
        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Utilisateur?> Register(Utilisateur utilisateur)
        {
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == utilisateur.Email))
            {
                return null;
            }

            // [NEW] Set a default role if none is provided
            if (string.IsNullOrEmpty(utilisateur.Role))
            {
                utilisateur.Role = "Spectateur";
            }

            utilisateur.MotDePasse = BCrypt.Net.BCrypt.HashPassword(utilisateur.MotDePasse);

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
            return utilisateur;
        }

        // [CHANGE] Return Type is now string? (The Token) instead of Utilisateur?
        public async Task<string?> Login(string email, string password)
        {
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.MotDePasse))
            {
                return null;
            }

            // [CHANGE] Return the token instead of the user object
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Utilisateur user)
        {
            // FIX: Add '?? ""' to handle possible nulls, though we know it exists
            var keyString = _configuration["Jwt:Key"] ?? "MYSUPERSECRETKEY_FALLBACK_123456";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "Spectateur")
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}