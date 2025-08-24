using Microsoft.IdentityModel.Tokens;
using security_service.Database.Entities;
using security_service.Utils.Classes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace security_service.Services
{
    public class TokenService
    {
        private const string SecretKey = "super_secret_key_1234567891112131415";

        private const int TokenExpirationMinutes = 60;

        public string GenerateToken(string username, string userRole)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, userRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "example-app",
                audience: "example-app-users",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(TokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(int userId, string username, string role)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Username = username,
                Role = role,
                TokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            return refreshToken;
        }
    }
}
