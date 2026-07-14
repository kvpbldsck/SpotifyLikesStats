using Spectre.Console;
using Spectre.Console.Rendering;

namespace View.SpectreConsole;

internal sealed class ProgressBarCounterColumn(Style? style = null, Style? completedStyle = null) : ProgressColumn
{
    private const double Tolerance = 0.0000001;

    private Style Style { get; } = style ?? Style.Plain;

    /// <summary>
    /// Gets or sets the style for a completed task.
    /// </summary>
    private Style CompletedStyle { get; } = completedStyle ?? Color.Green;

    /// <inheritdoc/>
    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        var style = !task.IsIndeterminate && Math.Abs(task.Value - task.MaxValue) < Tolerance
            ? CompletedStyle
            : Style;
        return task.IsIndeterminate
            ? new Text("--/--", style).RightJustified()
            : new Text($"{task.Value}/{task.MaxValue}", style).RightJustified();
    }
}