using Application.Contracts;
using Spectre.Console;

namespace View.SpectreConsole;

internal sealed class SpectreProgressTracker(ProgressContext context, ProgressTask task) : IProgressTracker
{
    public void SetGoal(int goal)
    {
        if (CanChangeGoal())
        {
            task.IsIndeterminate(false);
            task.MaxValue(goal);
        }
    }

    public void IncreaseProgress(int value)
    {
        if (CanChange())
        {
            task.Increment(value);
        }
    }

    private bool CanChange() =>
        !context.IsFinished
        && !task.IsFinished;

    private bool CanChangeGoal() =>
        CanChange()
        && task.IsIndeterminate;
}
