namespace Spotify.Models;

internal sealed class Settings
{
    public const string SectionName = "Spotify";

    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string RedirectUrl { get; init; }
    public required string Scope { get; init; }
    public required string AuthUrl { get; init; }
    public required string AccessTokenUrl { get; init; }
    public required string GetUserSavedTracksUrl { get; init; }
}
