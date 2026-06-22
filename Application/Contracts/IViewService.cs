using Stats.Models;

namespace Application.Contracts;

public interface IViewService
{
    void ShowStats(StatsDto stats);

    Task<T> ShowProgressBarAsync<T>(string progressDescription, Func<IProgressTracker, Task<T>> whileProgressRunsAsync);
}