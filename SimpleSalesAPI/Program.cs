
using MySqlConnector;
using SimpleSalesAPI.Application;
using SimpleSalesAPI.Application.Middleware;
using SimpleSalesAPI.Infrastructure;

namespace SimpleSalesAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddApplication();
			builder.Services.AddInfrastructure(builder.Configuration);
			
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
				});
			});
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseMiddleware<ValidationMiddleware>();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
