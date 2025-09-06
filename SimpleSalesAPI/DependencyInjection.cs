using System.Reflection;

namespace SimpleSalesAPI
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			//automapper e os serviços 

			return services;
		}
	}
}
