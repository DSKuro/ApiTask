using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;

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

        public static async Task<object> GetDataWithJSON(string uri, string? param, Type CastType)
        {
            HttpResponseMessage response;
                response = await GetResponse(uri, param);
            object? data = await response.Content.ReadFromJsonAsync(CastType);
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return data;
        }

        private static async Task<HttpResponseMessage> GetResponse(string uri, string? param)
        {
            string path = GetFullPath(uri, param);
            HttpResponseMessage? response = await client.GetAsync(path);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new HttpRequestException("Get an error from site");
            }
            return response;
        }

        private static string GetFullPath(string uri, string? param)
        {
            string fullUri = uri[uri.Length - 1] == '/' ? uri + "/" : uri;
            string path = new StringBuilder(fullUri + "/" + param).ToString();
            return path;
        }
    }
}
