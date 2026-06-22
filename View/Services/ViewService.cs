using Application.Contracts;
using Misc;
using Spectre.Console;
using Stats.Models;
using View.SpectreConsole;

namespace View.Services;

internal sealed class ViewService : IViewService
{
    private const int WidthBreakpoint = 250;
    private const int VerticalPadding = 5;
    private const string TopArtistsLabel = "Top artists";
    private const string TopAlbumsLabel = "Top albums";
    private static readonly Color[] _availableColors = [Color.Blue, Color.Yellow, Color.Red, Color.Green, Color.Violet];

    public async Task<T> ShowProgressBarAsync<T>(string progressDescription, Func<IProgressTracker, Task<T>> whileProgressRunsAsync) =>
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn() { CompletedStyle = Color.Blue },
                new ProgressBarCounterColumn())
            .AutoClear(true)
            .HideCompleted(true)
            .StartAsync(
                async ctx =>
                {
                    var task = ctx.AddTask(progressDescription)
                        .IsIndeterminate();
                    var tracker = new SpectreProgressTracker(ctx, task);

                    return await whileProgressRunsAsync(tracker);
                });

    public void ShowStats(StatsDto stats)
    {
        AnsiConsole.Write(
            new Columns(
                PreparePanel(stats.TopArtists, TopArtistsLabel, PrepareArtistLabel),
                PreparePanel(stats.TopAlbums, TopAlbumsLabel, PrepareAlbumLabel))
            {
                Expand = true,
                Padding = new Padding(0, VerticalPadding)
            });
    }

    private static Panel PreparePanel<T>(IReadOnlyCollection<RatedItem<T>> itemsToShow, string header, Func<T, string> getLabel)
    {
        var chartItems = itemsToShow
            .Index()
            .Select(i => new BarChartItem(
                getLabel(i.Item.Item),
                i.Item.Rate,
                _availableColors[i.Index % _availableColors.Length]));

        var chart = new BarChart()
            .Label("by liked tracks")
            .AddItems(chartItems);

        return new Panel(chart)
            {
                Width = AnsiConsole.Profile.Width < WidthBreakpoint
                    ? AnsiConsole.Profile.Width
                    : (AnsiConsole.Profile.Width - VerticalPadding) / 2
            }
            .Header(header);
    }

    private static string PrepareArtistLabel(ArtistInfoDto artist) => artist.Name;
    private static string PrepareAlbumLabel(AlbumInfoDto album) => $"{album.Artists.Select(a => a.Name).Join(", ")} - {album.Name}";
}