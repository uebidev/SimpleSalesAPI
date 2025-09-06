using Serilog.Context;
using System.Diagnostics;

namespace SimpleSalesAPI.Middleware
{
	public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			var stopwatch = Stopwatch.StartNew();
			var correlationId = Guid.NewGuid().ToString("N")[..8];

			// Enriquecer contexto de logs com informações da request
			using (LogContext.PushProperty("CorrelationId", correlationId))
			using (LogContext.PushProperty("RequestPath", context.Request.Path))
			using (LogContext.PushProperty("RequestMethod", context.Request.Method))
			using (LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString()))
			using (LogContext.PushProperty("RemoteIP", context.Connection.RemoteIpAddress?.ToString()))
			{
				context.Response.Headers.Add("X-Correlation-ID", correlationId);

				logger.LogInformation("Request iniciada: {Method} {Path}",
					context.Request.Method, context.Request.Path);

				try
				{
					await next(context);
				}
				finally
				{
					stopwatch.Stop();
					var elapsed = stopwatch.ElapsedMilliseconds;

					var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error :
								  context.Response.StatusCode >= 400 ? LogLevel.Warning :
								  LogLevel.Information;

					logger.Log(logLevel,
						"Request finalizada: {Method} {Path} {StatusCode} em {ElapsedMs}ms",
						context.Request.Method,
						context.Request.Path,
						context.Response.StatusCode,
						elapsed);

					// Log de performance para requests lentas
					if (elapsed > 1000)
					{
						logger.LogWarning("Request lenta detectada: {Method} {Path} levou {ElapsedMs}ms",
							context.Request.Method, context.Request.Path, elapsed);
					}
				}
			}
		}
	}
}
