using System;
using Firepuma.Api.Abstractions.Actor;
using Firepuma.Api.Abstractions.Errors;
using Firepuma.Api.Common.Actor;
using Firepuma.Api.Common.Configure;
using Firepuma.Auditing.Abstractions.Tenants;
using Firepuma.Auditing.Client.Tenants;
using Firepuma.MicroServices.Auth.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.Auditing.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuditingServiceClient<TActor>(this IServiceCollection services,
            IConfigurationSection auditingConfigSection,
            IConfigurationSection tokenProviderConfigSection,
            bool isMultiTenant) where TActor : IActorIdentity
        {
            services.AddScoped<IAuditingService, AuditingService<TActor>>();
            services.ConfigureAndValidate<AuditingMicroServiceOptions>(auditingConfigSection.Bind);

            services.AddOpenIdConnectTokenProvider(tokenProviderConfigSection);

            services.AddScoped<IRemoteIpProvider, HttpContextRemoteIpProvider>();

            if (!isMultiTenant)
            {
                services.AddScoped<ITenantNameProviderHolder>(s => new SpecificTenantNameProviderHolder(new SpecificTenantNameProvider(null)));
            }

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                if (scope.ServiceProvider.GetService<IErrorReportingService>() == null)
                {
                    throw new Exception($"Please register IErrorReportingService service before calling {nameof(AddAuditingServiceClient)}");
                }
                if (scope.ServiceProvider.GetService<IActorProvider<TActor>>() == null)
                {
                    throw new Exception($"Please register IActorProvider<TActor> service before calling {nameof(AddAuditingServiceClient)}");
                }

                if (scope.ServiceProvider.GetService<ITenantNameProviderHolder>() == null)
                {
                    throw new Exception($"Please register {typeof(ITenantNameProviderHolder).FullName} service before " +
                                        $"calling {nameof(AddAuditingServiceClient)} (this is required because isMultiTenant==true)");
                }
            }
        }
    }
}