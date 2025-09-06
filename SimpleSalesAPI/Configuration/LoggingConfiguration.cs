using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace SimpleSalesAPI.Configuration
{
	public static class LoggingConfiguration
	{
		/// <summary>
		/// Configura Serilog com enrichers e sinks 
		/// </summary>
		public static void ConfigureSerilog(this WebApplicationBuilder builder)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information() 
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Reduz spam do framework
				.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)

				// ENRICHERS - Contexto adicional em cada log
				.Enrich.FromLogContext()
				.Enrich.WithEnvironmentName()
				.Enrich.WithMachineName()
				.Enrich.WithProcessId()
				.Enrich.WithThreadId()
				.Enrich.WithProperty("Application", "SimpleSalesAPI")
				.Enrich.WithProperty("Version", GetApplicationVersion())

				// CONSOLE SINK - Para desenvolvimento
				.WriteTo.Console(
					outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")

				// FILE SINK - Para produção e debugging
				.WriteTo.File(
					new CompactJsonFormatter(),
					path: "logs/simplesales-.log",
					rollingInterval: RollingInterval.Day,
					retainedFileCountLimit: 30,
					shared: true,
					flushToDiskInterval: TimeSpan.FromSeconds(1))

				// FILE SINK para erros críticos - Separado para facilitar monitoramento
				.WriteTo.File(
					path: "logs/errors/simplesales-errors-.log",
					restrictedToMinimumLevel: LogEventLevel.Error,
					rollingInterval: RollingInterval.Day,
					retainedFileCountLimit: 90,
					shared: true,
					outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")

				.CreateLogger();

			builder.Host.UseSerilog();
		}

		/// <summary>
		/// Obtém versão da aplicação
		/// </summary>
		private static string GetApplicationVersion()
		{
			return System.Reflection.Assembly.GetExecutingAssembly()
				.GetName().Version?.ToString() ?? "Unknown";
		}
	}

}
