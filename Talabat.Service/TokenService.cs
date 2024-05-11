using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.Service
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
		{
			//Body
			//1- Private calims
			var authenticationClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.GivenName, user.DisplayName),
				new Claim(ClaimTypes.Email, user.Email)
			};

			var roles = await userManager.GetRolesAsync(user);
			foreach (var role in roles)
				authenticationClaims.Add(new Claim(ClaimTypes.Role, role));

			var authenticationKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
				claims: authenticationClaims,
				signingCredentials: new SigningCredentials(authenticationKey, SecurityAlgorithms.Aes128CbcHmacSha256)
				);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
