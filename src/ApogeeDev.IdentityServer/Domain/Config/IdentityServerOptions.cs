using System;

namespace ApogeeDev.IdentityServer.Domain.Config
{
    public class IdentityServerOptions
    {
        public string SigningCredentialKey { get; set; }

        public byte[] GetSigningKeyBytes()
        {
            if (!string.IsNullOrWhiteSpace(SigningCredentialKey))
            {
                return Convert.FromBase64String(SigningCredentialKey);
            }
            throw new InvalidOperationException($"Null or empty {nameof(SigningCredentialKey)}");
        }
    }
}