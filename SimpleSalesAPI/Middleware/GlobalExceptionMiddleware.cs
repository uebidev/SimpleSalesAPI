using FluentValidation;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using InvalidOperationException = SimpleSalesAPI.Domain.Exceptions.InvalidOperationException;

namespace SimpleSalesAPI.Middleware
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception exception)
			{
				await HandleExceptionAsync(context, exception);
			}
		}

		/// <summary>
		/// Método central que direciona cada tipo de exception para o tratamento adequado
		/// </summary>
		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var traceId = context.TraceIdentifier;

			// Log da exception antes de processar
			LogException(exception, traceId, context.Request.Path);

			var response = exception switch
			{
				ValidationException validationEx => HandleValidationException(validationEx, traceId, context),
				NotFoundException notFoundEx => HandleNotFoundException(notFoundEx, traceId, context),
				InsufficientStockException stockEx => HandleInsufficientStockException(stockEx, traceId, context),
				InvalidOperationException invalidOpEx => HandleInvalidOperationException(invalidOpEx, traceId, context),
				BusinessException businessEx => HandleBusinessException(businessEx, traceId, context),
				UnauthorizedAccessException _ => HandleUnauthorizedException(traceId, context),
				ArgumentException argEx => HandleArgumentException(argEx, traceId, context),
				_ => HandleGenericException(exception, traceId, context)
			};

			await WriteResponseAsync(context, response);
		}

		/// <summary>
		/// Tratamento de ValidationException do FluentValidation
		/// </summary>
		private static ValidationErrorResponse HandleValidationException(
			ValidationException exception, string traceId, HttpContext context)
		{
			return new ValidationErrorResponse
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
				Title = "Dados de entrada inválidos",
				Status = (int)HttpStatusCode.BadRequest,
				Detail = "Um ou mais campos possuem valores inválidos",
				Instance = context.Request.Path,
				TraceId = traceId,
				Errors = exception.Errors
					.GroupBy(x => x.PropertyName)
					.ToDictionary(
						g => ToCamelCase(g.Key),
						g => g.Select(x => x.ErrorMessage).ToArray()
					)
			};
		}

		/// <summary>
		/// Tratamento de NotFoundException 
		/// </summary>
		private static ErrorResponse HandleNotFoundException(
			NotFoundException exception, string traceId, HttpContext context)
		{
			return new ErrorResponse
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
				Title = "Recurso não encontrado",
				Status = (int)HttpStatusCode.NotFound,
				Detail = exception.Message,
				Instance = context.Request.Path,
				TraceId = traceId,
				Extensions = new Dictionary<string, object>
				{
					["resourceType"] = exception.ResourceType,
					["resourceId"] = exception.ResourceId
				}
			};
		}

		/// <summary>
		/// Tratamento específico para estoque insuficiente
		/// </summary>
		private static ErrorResponse HandleInsufficientStockException(
			InsufficientStockException exception, string traceId, HttpContext context)
		{
			return new ErrorResponse
			{
				Type = "https://example.com/problems/insufficient-stock",
				Title = "Estoque insuficiente",
				Status = (int)HttpStatusCode.BadRequest,
				Detail = exception.Message,
				Instance = context.Request.Path,
				TraceId = traceId,
				Extensions = new Dictionary<string, object>
				{
					["productName"] = exception.ProductName,
					["requestedQuantity"] = exception.RequestedQuantity,
					["availableQuantity"] = exception.AvailableQuantity,
					["errorCode"] = exception.ErrorCode
				}
			};
		}

		/// <summary>
		/// Tratamento para operações inválidas baseadas em estado
		/// </summary>
		private static ErrorResponse HandleInvalidOperationException(
			InvalidOperationException exception, string traceId, HttpContext context)
		{
			return new ErrorResponse
			{
				Type = "https://example.com/problems/invalid-operation",
				Title = "Operação não permitida",
				Status = (int)HttpStatusCode.BadRequest,
				Detail = exception.Message,
				Instance = context.Request.Path,
				TraceId = traceId,
				Extensions = new Dictionary<string, object>
				{
					["currentState"] = exception.CurrentState,
					["attemptedOperation"] = exception.AttemptedOperation,
					["errorCode"] = exception.ErrorCode
				}
			};
		}

		/// <summary>
		/// Tratamento genérico para BusinessException
		/// </summary>
		private static ErrorResponse HandleBusinessException(
			BusinessException exception, string traceId, HttpContext context)
		{
			return new ErrorResponse
			{
				Type = "https://example.com/problems/business-rule",
				Title = "Regra de negócio violada",
				Status = (int)HttpStatusCode.BadRequest,
				Detail = exception.Message,
				Instance = context.Request.Path,
				TraceId = traceId,
				Extensions = new Dictionary<string, object>
				{
					["errorCode"] = exception.ErrorCode,
					["details"] = exception.Details ?? new { }
				}
			};
		}

		/// <summary>
		/// Tratamento para acesso não autorizado
		/// </summary>
		private static ErrorResponse HandleUnauthorizedException(string traceId, HttpContext context)
		{
			return new ErrorResponse
			{
				Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
				Title = "Acesso não autorizado",
				Status = (int)HttpStatusCode.Unauthorized,
				Detail = "Credenciais inválidas ou ausentes",
				Instance = context.Request.Path,
				TraceId = traceId
			};
		}

		/// <summary>
		/// Tratamento para ArgumentException
		/// </summary>
		private static ErrorResponse HandleArgumentException(
			ArgumentException exception, string traceId, HttpContext context)
		{
			return new ErrorResponse
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
				Title = "Parâmetro inválido",
				Status = (int)HttpStatusCode.BadRequest,
				Detail = exception.Message,
				Instance = context.Request.Path,
				TraceId = traceId
			};
		}

		/// <summary>
		/// Tratamento genérico para exceptions não mapeadas
		/// </summary>
		private static ErrorResponse HandleGenericException(
			Exception exception, string traceId, HttpContext context)
		{
			return new ErrorResponse
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
				Title = "Erro interno do servidor",
				Status = (int)HttpStatusCode.InternalServerError,
				Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
				Instance = context.Request.Path,
				TraceId = traceId
			};
		}

		/// <summary>
		/// Log estruturado das exceptions
		/// </summary>
		private void LogException(Exception exception, string traceId, string requestPath)
		{
			var logLevel = exception switch
			{
				ValidationException => LogLevel.Warning,
				NotFoundException => LogLevel.Warning,
				BusinessException => LogLevel.Warning,
				ArgumentException => LogLevel.Warning,
				_ => LogLevel.Error
			};

			_logger.Log(logLevel, exception,
				"Exception occurred. TraceId: {TraceId}, Path: {RequestPath}, ExceptionType: {ExceptionType}",
				traceId, requestPath, exception.GetType().Name);
		}

		/// <summary>
		/// Escreve a response HTTP estruturada
		/// </summary>
		private static async Task WriteResponseAsync(HttpContext context, ErrorResponse response)
		{
			context.Response.ContentType = "application/problem+json";
			context.Response.StatusCode = response.Status;

			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};

			var json = JsonSerializer.Serialize(response, options);
			await context.Response.WriteAsync(json);
		}

		/// <summary>
		/// Converte PropertyName para camelCase
		/// </summary>
		private static string ToCamelCase(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			return char.ToLowerInvariant(input[0]) + input[1..];
		}
	}
}
