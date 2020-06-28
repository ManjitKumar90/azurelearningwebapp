using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSALAuthentication
{
    class Program
    {
        static void Main(string[] args)
        {
            var secretUri = "https://intellipatvault.vault.azure.net/secrets/ConnectionString";
            //KeyVaultClient client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            //var secret = client.GetSecretAsync(secretUri).ConfigureAwait(false).GetAwaiter().GetResult();

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var client1 = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret1 = client1.GetSecretAsync(secretUri).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine(secret1.Value);
            //GetAccessToken().GetAwaiter().GetResult();
        }

        private static async Task<string> GetAccessToken(string a, string b, string c)
        {
            const string ClientId = "e342d80c-b41c-42bf-b3d8-e5c43baf991a";
            const string TennatId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
            const string ClientSecret = "_q-_11fCty581gIIiQF6_C81C~RMM4Au2Q";

            var app = ConfidentialClientApplicationBuilder.Create(ClientId).WithAuthority(AzureCloudInstance.AzurePublic, TennatId)
                    .WithClientSecret(ClientSecret).Build();

            var scope = new List<string>() { "https://vault.azure.net/.default" };
            AuthenticationResult result = await app.AcquireTokenForClient(scope).ExecuteAsync();

            return result.AccessToken;
        }
    }
}
