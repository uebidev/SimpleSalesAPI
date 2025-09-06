using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimpleSalesAPI.Application.Services;
using SimpleSalesAPI.Application.Services.Interfaces;
using System.Reflection;

namespace SimpleSalesAPI.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			
			services.AddScoped<IVendaService, VendaService>();
			
			services.AddScoped<IProdutoService, ProdutoService>();

			services.AddScoped<IClienteService, ClienteService>();

			services.AddScoped<ICategoriaService, CategoriaService>();
		

			services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

			return services;
		}
	}
}
