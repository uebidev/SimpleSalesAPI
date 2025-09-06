using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleSalesAPI.Infrastructure.Data.Context;
using SimpleSalesAPI.Infrastructure.Data.Repositories;
using SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<AppDbContext>(options =>
			{
				options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
					mySqlOptions =>
					{
						mySqlOptions.MigrationsAssembly("SimpleSalesAPI.Infrastructure");
						mySqlOptions.EnableRetryOnFailure(
							maxRetryCount: 3);
					});
				if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
				{
					options.EnableSensitiveDataLogging();
					options.EnableDetailedErrors();
				}
			});

			services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
