using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Middleware
{
	/// <summary>
	/// Middleware para capturar ValidationExceptions - Porque erro bem tratado é profissionalismo!
	/// </summary>
	public class ValidationMiddleware
	{
		private readonly RequestDelegate _next;

		public ValidationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (ValidationException ex)
			{
				await HandleValidationExceptionAsync(context, ex);
			}
		}

		/// <summary>
		/// Trata ValidationException
		/// </summary>
		private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
		{
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			context.Response.ContentType = "application/json";

			var response = new
			{
				type = "ValidationError",
				title = "Dados inválidos",
				status = 400,
				errors = ex.Errors.GroupBy(e => e.PropertyName)
					.ToDictionary(
						g => g.Key,
						g => g.Select(e => new {
							message = e.ErrorMessage,
							code = e.ErrorCode
						}).ToArray()
					)
			};

			var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});

			await context.Response.WriteAsync(jsonResponse);
		}
	}
}
