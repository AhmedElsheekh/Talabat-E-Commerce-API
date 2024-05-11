using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
	public static class AppIdentityDbContextSeed
	{
		public static async Task SeedUserAsync(UserManager<AppUser> userManager)
		{
			if(!userManager.Users.Any())
			{
				var user = new AppUser
				{
					DisplayName = "Aliaa Tarek",
					Email = "aliaatarek.route@gmail.com",
					UserName = "aliaatarek.route",
					PhoneNumber = "01245254256"
				};

				await userManager.CreateAsync(user, "Pa$$w0rd");
			}
		}
	}
}
