using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace ApogeeDev.IdServer.Core.Config.Default
{
    public static partial class ApplicationData
    {
        public static IEnumerable<ApiResource> Apis => new List<ApiResource>
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
                        DisplayName = "Full access to API 2",
                        UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email }
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