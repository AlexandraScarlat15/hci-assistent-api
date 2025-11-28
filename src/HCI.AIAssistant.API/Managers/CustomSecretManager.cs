using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace HCI.AIAssistant.API.Managers
{
    public class CustomSecretManager : KeyVaultSecretManager
    {
        private readonly string _prefix;

        public CustomSecretManager(string prefix)
        {
            // prefix + "-"  (EXACT cum cere laboratorul)
            _prefix = $"{prefix}-";
        }

        public override bool Load(SecretProperties secret)
            => secret.Name.StartsWith(_prefix);

        public override string GetKey(KeyVaultSecret secret)
            => secret.Name[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
    }
}
