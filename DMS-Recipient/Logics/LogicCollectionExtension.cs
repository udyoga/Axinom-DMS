using DMS_recipient.Logics.Implementation;
using DMS_recipient.Logics.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DMS_recipient.Logics
{
    public static class LogicCollectionExtension
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {           
            services.AddScoped<IEncryptionLogic, EncryptionLogic>();
            services.AddScoped<IDatabaseLogic, DatabaseLogic>();
            return services;
        }
    }
}
