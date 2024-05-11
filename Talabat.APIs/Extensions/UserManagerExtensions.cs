using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Extensions
{
	public static class UserManagerExtensions
	{
		public static async Task<AppUser?> FindUserWithAddressAsync(this UserManager<AppUser> userManager,
			ClaimsPrincipal User)
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var user = await userManager.Users.Where(U => U.Email == email).Include(U => U.Address).FirstOrDefaultAsync();

			return user;
		}
	}
}
