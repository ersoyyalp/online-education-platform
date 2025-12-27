using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEducation.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(UserAuthDto user)
        {
            var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
    new Claim("email", user.Email),
    new Claim(ClaimTypes.Role, user.Role)
};

            if (user.InstructorId.HasValue)
                claims.Add(new Claim("instructorId", user.InstructorId.Value.ToString()));

            if (user.ParticipantId.HasValue)
                claims.Add(new Claim("participantId", user.ParticipantId.Value.ToString()));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpireMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
