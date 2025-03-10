using WebPortal.BusinessLogic.Interfaces;
using WebPortal.BusinessLogic.Repositories;
using WebPortal.Utilities;

namespace WebPortal.Extensions
{
	public static class InterfaceScopeExtensions
	{
		public static IServiceCollection AddInterfaceScopeServices(this IServiceCollection services, IConfiguration configuration)
		{

			#region Scops

			services.AddScoped<FirebaseUtility>();
			services.AddScoped<FirebaseStorageUtility>();
			services.AddSingleton<FirebaseUtility>();
			services.AddHostedService<PlanExpirationChecker>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IChannelRepository, ChannelRepository>();
			services.AddScoped<ICodeRepository, CodeRepository>();
			services.AddScoped<IPlanRepository, PlanRepository>();

			#endregion

			return services;
		}
	}
}
