using Misc;

namespace Stats.Models;

public sealed record TrackInfoDto(
    string Id,
    string Name,
    ArtistInfoDto[] Artists,
    AlbumInfoDto Album)
{
    public override string ToString() => $"{Artists.Select(a => a.ToString()).Join(", ")} - {Name} (from {Album.Name} album)";
}

public sealed record AlbumInfoDto(string Id, string Name, ArtistInfoDto[] Artists)
{
    public override string ToString() => $"'{Name}' by {Artists.Select(a => a.ToString()).Join(", ")}";
}

public sealed record ArtistInfoDto(string Id, string Name)
{
    public override string ToString() => Name;
}
