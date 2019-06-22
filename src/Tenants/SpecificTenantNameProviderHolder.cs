using Firepuma.Auditing.Abstractions.Tenants;

namespace Firepuma.Auditing.Client.Tenants
{
    public class SpecificTenantNameProviderHolder : ITenantNameProviderHolder
    {
        public ITenantNameProvider Provider { get; set; }

        public SpecificTenantNameProviderHolder(ITenantNameProvider provider)
        {
            Provider = provider;
        }
    }
}