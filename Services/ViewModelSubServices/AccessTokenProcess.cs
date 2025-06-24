using ApiTask.Models;
using System.Threading.Tasks;

namespace ApiTask.Services.ViewModelSubServices
{
    public class AccessTokenProcess
    {
        private static readonly string Key = "api";
        private static readonly string AuthSite = "auth";
        private static readonly string AuthHeader = "header";

        public Token AccessToken { get; private set; }
        
        public async Task ApplyTokenImpl()
        {
            await GetToken();
            Http.AddHeader(ReadConfiguration.GetValueByKeyFromSecrets(AuthHeader), AccessToken.AccessToken);
        }

        private async Task GetToken()
        {
            (string apiKey, string authKey) = GetKeysFromConfig();
            AccessToken = (Token)await Http.GetDataWithJSON(GetFullTokenPath(authKey, apiKey), typeof(Token));
        }

        private Http.QueryBuilder GetFullTokenPath(string uri, string param)
        {
            Http.QueryBuilder path = new Http.QueryBuilder(uri);
            path.SetParam(param);
            return path;
        }

        private (string, string) GetKeysFromConfig()
        {
            string apiKey = ReadConfiguration.GetValueByKeyFromSecrets(Key);
            string authKey = ReadConfiguration.GetValueByKeyFromConfiguration(AuthSite);
            return (apiKey, authKey);
        }
    }
}
