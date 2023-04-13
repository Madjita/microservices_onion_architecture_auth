using System;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using AuthDAL.Entities;

namespace AuthBLL.Bearer.Auth.Auth.JWT
{
    public static class UserHelper
    {
        public static string GenerateJwtToken(this IConfiguration configuration, IOptions<AuthOptions> authOptions, Account account)
        {
            
            var authParametrs = authOptions.Value;

            var securityKey = authParametrs.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email,account.Email),
                new Claim(JwtRegisteredClaimNames.Sub,account.Id.ToString()),
                new Claim("role",account.Role.Name),
            };


            var token = new JwtSecurityToken(
                authParametrs.Issuer,
                authParametrs.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParametrs.TokenLifeTime),
                signingCredentials: credentials
            );

            string StrToken = new JwtSecurityTokenHandler().WriteToken(token);

            return StrToken;
        }
    }
}

