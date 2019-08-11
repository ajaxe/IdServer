using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace ApogeeDev.IdServer.Core.Config.Default
{
    public static partial class ApplicationData
    {
        public static IEnumerable<IdentityResource> Identities => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };
    }
}