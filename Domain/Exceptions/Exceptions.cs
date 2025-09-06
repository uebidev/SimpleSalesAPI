using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Domain.Exceptions
{
	/// <summary>
	/// Exception base para regras de negócio violadas
	/// </summary>
	public class BusinessException : Exception
	{
		public string ErrorCode { get; }
		public object? Details { get; }

		public BusinessException(string message, string errorCode = "BUSINESS_RULE_VIOLATION", object? details = null)
			: base(message)
		{
			ErrorCode = errorCode;
			Details = details;
		}
	}
	/// <summary>
	/// Exception para recursos não encontrados
	/// </summary>
	public class NotFoundException : Exception
	{
		public string ResourceType { get; }
		public object ResourceId { get; }

		public NotFoundException(string resourceType, object resourceId)
			: base($"{resourceType} com ID '{resourceId}' não foi encontrado")
		{
			ResourceType = resourceType;
			ResourceId = resourceId;
		}

		public NotFoundException(string message) : base(message)
		{
			ResourceType = "Resource";
			ResourceId = "Unknown";
		}
	}
	/// <summary>
	/// Exception para estoque insuficiente - Mais específica que BusinessException
	/// </summary>
	public class InsufficientStockException : BusinessException
	{
		public string ProductName { get; }
		public int RequestedQuantity { get; }
		public int AvailableQuantity { get; }

		public InsufficientStockException(string productName, int requested, int available)
			: base($"Estoque insuficiente para '{productName}'. Solicitado: {requested}, Disponível: {available}",
				   "INSUFFICIENT_STOCK")
		{
			ProductName = productName;
			RequestedQuantity = requested;
			AvailableQuantity = available;
		}
	}
	/// <summary>
	/// Exception para operações não permitidas devido ao estado atual
	/// </summary>
	public class InvalidOperationException : BusinessException
	{
		public string CurrentState { get; }
		public string AttemptedOperation { get; }

		public InvalidOperationException(string currentState, string attemptedOperation, string message)
			: base(message, "INVALID_OPERATION")
		{
			CurrentState = currentState;
			AttemptedOperation = attemptedOperation;
		}
	}

}
