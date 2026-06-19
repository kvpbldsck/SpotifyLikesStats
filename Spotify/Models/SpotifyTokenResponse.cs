using System.Text.Json.Serialization;

namespace Spotify.Models;

public sealed class SpotifyTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
}