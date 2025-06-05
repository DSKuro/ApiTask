using System.Text.Json.Serialization;

namespace ApiTask.Models
{
    public record Token
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
