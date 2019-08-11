using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace ApogeeDev.IdServer.Core.Config.Default
{
    public static partial class ApplicationData
    {
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "test_mvc.client",
                ClientName = "test mvc client",
                ClientSecrets = { new Secret ("secret".Sha256 ()) },

                AllowedGrantTypes = GrantTypes.Hybrid,

                RedirectUris = { "http://localhost:5003/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },
                FrontChannelLogoutUri = "http://localhost:5003/signout-oidc",

                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,

                    "api1",
                    "api2.read_only"
                },

                AllowOfflineAccess = true,
                BackChannelLogoutSessionRequired = true,
                BackChannelLogoutUri = "http://localhost:5003/logout"
            }
        };
    }
}