using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace Lesson2.Services
{
    public class TokenGenerate : ITokenGenerate
    {
        private readonly IConfiguration _config;

        public TokenGenerate(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string username, int userId)
        {
            var secret = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.NameIdentifier, userId.ToString())},
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
