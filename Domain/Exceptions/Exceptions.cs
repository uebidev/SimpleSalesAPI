using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Domain.Exceptions
{
	public class BusinessException(string message) : Exception(message)
	{
	}
	public class NotFoundException(string message) : Exception(message)
	{
	}
	public class InsufficientStockException(string produto, int solicitado, int disponivel)
		: BusinessException($"Estoque insuficiente para '{produto}'. Solicitado: {solicitado}, Disponível: {disponivel}")
	{
	}

}
