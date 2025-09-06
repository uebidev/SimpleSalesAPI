
using Microsoft.AspNetCore.Builder;
using MySqlConnector;
using Serilog;
using SimpleSalesAPI.Application;
using SimpleSalesAPI.Configuration;
using SimpleSalesAPI.Infrastructure;
using SimpleSalesAPI.Middleware;

namespace SimpleSalesAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.ConfigureSerilog();

			try
			{
				Log.Information("Iniciando SimpleSalesAPI");

				builder.Services.AddControllers();
				builder.Services.AddEndpointsApiExplorer();
				builder.Services.AddSwaggerGen();
			
				builder.Services.AddApplication();
				builder.Services.AddInfrastructure(builder.Configuration);

				// CORS
				builder.Services.AddCors(options =>
				{
					options.AddPolicy("AllowAll", policy =>
					{
						policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
					});
				});
				var app = builder.Build();

				if (app.Environment.IsDevelopment())
				{
					app.UseSwagger();
					app.UseSwaggerUI();
				}

				app.UseHttpsRedirection();

				app.UseMiddleware<RequestLoggingMiddleware>();
				app.UseMiddleware<GlobalExceptionMiddleware>();

				app.UseCors("AllowAll");
				app.UseAuthorization();
				app.MapControllers();

				Log.Information("SimpleSalesAPI configurada com sucesso. Iniciando servidor...");

				app.Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Falha fatal na inicialização da aplicação");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
	}
}
