using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helper;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Add services to the container
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<StoreDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			builder.Services.AddDbContext<AppIdentityDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
			});

			builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
			{
				var connection = builder.Configuration.GetConnectionString("RedisConnection");
				return ConnectionMultiplexer.Connect(connection);
			});

			builder.Services.AddApplicationServices();
			builder.Services.AddIdentityServices(builder.Configuration);
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("MyPolicy", policyOptions =>
				{
					policyOptions.AllowAnyHeader();
					policyOptions.AllowAnyMethod();
					policyOptions.WithOrigins(builder.Configuration["FrontBaseUrl"]);
				});
			});
			#endregion


			var app = builder.Build();

			#region Update Database
			//Container Have All Scooped Services
			using var scope = app.Services.CreateScope();

			//Catch Services Itself
			var services = scope.ServiceProvider;

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();

			try
			{
				//Get Required Service
				var dbContext = services.GetRequiredService<StoreDbContext>();
				await dbContext.Database.MigrateAsync();

				var identityDbContext = services.GetRequiredService<AppIdentityDbContext>();			
				await identityDbContext.Database.MigrateAsync();

				await StoreDbContextSeed.SeedAsync(dbContext);

				var userManager = services.GetRequiredService<UserManager<AppUser>>();
				await AppIdentityDbContextSeed.SeedUserAsync(userManager);
			}
			catch(Exception ex)
			{
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "Error While Making Migrations On Database");
			}
			#endregion

			#region Configure the HTTP request pipeline
			if (app.Environment.IsDevelopment())
			{
				app.UseMiddleware<ExceptionMiddleware>();
				app.UseSwaggerMiddlewares();
			}
			app.UseStaticFiles();
			app.UseStatusCodePagesWithReExecute("/errors/{0}");
			app.UseHttpsRedirection();
			app.UseCors();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			#endregion


			app.Run();
		}
	}
}