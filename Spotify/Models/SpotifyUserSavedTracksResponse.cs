using System.Text.Json.Serialization;

namespace Spotify.Models;

internal sealed class SpotifyUserSavedTracksResponse
{
    [JsonPropertyName("next")]
    public string? NextBunch { get; init; }

    [JsonPropertyName("total")]
    public int TotalSavedTracks { get; init; }

    [JsonPropertyName("items")]
    public SpotifySavedTrack[] SavedTracks { get; set; }
}

internal sealed class SpotifySavedTrack
{
    [JsonPropertyName("added_at")]
    public DateTime AddedAt { get; set; }

    [JsonPropertyName("track")]
    public SpotifyTrack Track { get; set; }
}

internal sealed class SpotifyTrack
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }

    [JsonPropertyName("artists")]
    public SpotifySimplifiedArtist[] Artists { get; set; }

    [JsonPropertyName("album")]
    public SpotifySimplifiedAlbum Album { get; set; }
}

internal sealed class SpotifySimplifiedArtist
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}

internal sealed class SpotifySimplifiedAlbum
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("artists")]
    public SpotifySimplifiedArtist[] Artists { get; set; }
}