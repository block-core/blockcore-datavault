using Microsoft.AspNetCore.Authentication;

namespace Blockcore.DataVault.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "API Key";

        public string Scheme => DefaultScheme;

        public string AuthenticationType = DefaultScheme;
    }
}
