using DMS_Sender.Logics.Implementation;
using DMS_Sender.Logics.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DMS_Sender.Logics
{
    public static class LogicCollectionExtension
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IExtractLogic, ExtractLogic>();
            services.AddScoped<IEncryptionLogic, EncryptionLogic>();
            services.AddScoped<IRecipientAPILogic, RecipientAPILogic>();
            return services;
        }
    }
}
