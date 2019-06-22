using Firepuma.Auditing.Abstractions.Tenants;

namespace Firepuma.Auditing.Client.Tenants
{
    public class SpecificTenantNameProvider : ITenantNameProvider
    {
        private readonly string _tenant;

        public SpecificTenantNameProvider(string tenant)
        {
            _tenant = tenant;
        }

        public string Name => _tenant;
    }
}