using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace ApogeeDev.ConfigDataUpdater
{
    public class ApiResourceConfigurator
    {
        public IEnumerable<ApiResource> Data() => new List<ApiResource>
        {
            new ApiResource("api1", "Some API 1"),
            new ApiResource
            {
                Name = "api2",
                ApiSecrets = { new Secret("secret".Sha256()) },
                UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email },
                Scopes = {
                    new Scope()
                    {
                        Name = "api2.full_access",
                        DisplayName = "Full access to aPI 2"
                    },
                    new Scope()
                    {
                        Name = "api2.read_only",
                        DisplayName = "Read-only access to API 2"
                    }
                }
            }
        };
    }
}