using System.Collections.Generic;
using IdentityServer4.Models;

namespace ApogeeDev.ConfigDataUpdater
{
    public class IdentityResourceConfigurator
    {
        public IEnumerable<IdentityResource> Data() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };
    }
}