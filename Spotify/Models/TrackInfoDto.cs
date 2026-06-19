namespace Spotify.Models;

public sealed record TrackInfoDto(string Id, string Name)
{
    public ArtistInfoDto[] Artists { get; set; }
    public AlbumInfoDto? Album { get; set; }
}

public sealed record AlbumInfoDto(string Id, string Name)
{
    public ArtistInfoDto? Artist { get; set; }
}

public sealed record ArtistInfoDto(string Id, string Name);
