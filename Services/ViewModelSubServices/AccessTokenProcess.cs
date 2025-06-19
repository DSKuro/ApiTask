using ApiTask.Models;
using System.Threading.Tasks;

namespace ApiTask.Services.ViewModelSubServices
{
    public static class AccessTokenProcess
    {
        private static readonly string Key = "api";
        private static readonly string AuthSite = "auth";
        private static readonly string AuthHeader = "header";

        public static Token AccessToken { get; private set; }
        
        public static async Task ApplyTokenImpl()
        {
            await GetToken();
            Http.AddHeader(ReadConfiguration.GetValueByKeyFromSecrets(AuthHeader), AccessToken.AccessToken);
        }

        private static async Task GetToken()
        {
            (string apiKey, string authKey) = GetKeysFromConfig();
            AccessToken = (Token)await Http.GetDataWithJSON(GetFullTokenPath(authKey, apiKey), typeof(Token));
        }

        private static Http.QueryBuilder GetFullTokenPath(string uri, string param)
        {
            Http.QueryBuilder path = new Http.QueryBuilder(uri);
            path.SetParam(param);
            return path;
        }

        private static (string, string) GetKeysFromConfig()
        {
            string apiKey = ReadConfiguration.GetValueByKeyFromSecrets(Key);
            string authKey = ReadConfiguration.GetValueByKeyFromConfiguration(AuthSite);
            return (apiKey, authKey);
        }
    }
}
