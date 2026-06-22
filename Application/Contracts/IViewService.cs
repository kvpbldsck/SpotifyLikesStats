using Stats.Models;

namespace Application.Contracts;

public interface IViewService
{
    void ShowStats(StatsDto stats);

    Task<T> ShowProgressBarAsync<T>(Func<SetProgressBarGoal, IncrementProgressBar, Task<T>> whileProgressRunsAsync);
}

