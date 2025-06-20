using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using System;
using System.Net.Http.Headers;

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

        public static void AddHeader(string header, string value)
        {
            client.DefaultRequestHeaders.Add(header, value);
        }

        public static async Task<object> GetDataWithJSON(QueryBuilder path, Type castType)
        {
            try
            {
                return await GetDataWithJSONImpl(path, castType);
            }
            catch (UriFormatException ex)
            {
                throw new Exceptions.HttpException("Неправильный Uri адрес");
            }
            catch (HttpRequestException ex)
            {
                throw new Exceptions.HttpException("Ошибка ответа сайта");
            }
            catch (OperationCanceledException ex)
            {
                throw new Exceptions.HttpException("Ошибка чтения");
            }
            catch (ArgumentNullException ex)
            {
                throw new Exceptions.HttpException("Данных нет");
            }
            catch (Exception ex)
            {
                throw new Exceptions.HttpException("Ошибка подключения");
            }
        }

        private static async Task<object> GetDataWithJSONImpl(QueryBuilder path, Type castType)
        {
            HttpResponseMessage response;
                response = await GetResponse(path);
            object? data = await response.Content.ReadFromJsonAsync(castType);
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return data;
        }

        private static async Task<HttpResponseMessage> GetResponse(QueryBuilder path)
        {
            HttpResponseMessage? response = await client.GetAsync(path.query);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest
                || response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new HttpRequestException("Get an error from site");
            }
            return response;
        }

        public class QueryBuilder
        {
            public string query { get; private set; } 

            public QueryBuilder(string uri)
            {
                InitializeQuery(uri);
            }

            private void InitializeQuery(string uri)
            {
                query = uri;
                if (query[query.Length - 1] == '/')
                {
                    query = query.Remove(query.Length, 1);
                }
            }

            public void SetParam(string param)
            {
                query += "/" + param;
            }

            public void SetQuery(string name, string queryParam)
            {
                if (query.Contains("?"))
                {
                    query += "&";
                }
                else
                {
                    query += "?";
                }
                query += name + "=" + queryParam;
            }
        }
    }
}
