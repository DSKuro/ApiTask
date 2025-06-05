using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace ApiTask.Services
{
    public class Http
    {
        private static readonly string AcceptValue = "application/json";
        private static HttpClient client = new HttpClient();

        static Http() 
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptValue));
        }

        public static void SetAuthorizationHeader(string token)
        {
            client.DefaultRequestHeaders.Add("Accesstoken", token);
        }

        public static async Task<object> GetDataWithJSON(string uri, string? param, string? query, Type CastType)
        {
            HttpResponseMessage response;
                response = await GetResponse(uri, param, query);
            object? data = await response.Content.ReadFromJsonAsync(CastType);
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return data;
        }

        private static async Task<HttpResponseMessage> GetResponse(string uri, string? param, string? query)
        {
            string path = GetFullPath(uri, param, query);
            HttpResponseMessage? response = await client.GetAsync(path);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new HttpRequestException("Get an error from site");
            }
            return response;
        }

        private static string GetFullPath(string uri, string? param, string? query)
        {
            // может вынести в отдельный класс
            string fullUri = uri[uri.Length - 1] == '/' ? uri.Substring(0, uri.Length - 1) : uri;
            StringBuilder fullPath = new StringBuilder(fullUri);

            if (param != null)
            {
                fullPath.Append("/" + param);
            }

            if (query != null)
            {
                fullPath.Append("?" + query);
            }

            return fullPath.ToString();
        }
    }
}
