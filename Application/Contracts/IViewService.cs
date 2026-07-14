using Application.Models;
using CSharpFunctionalExtensions;
using Stats.Models;

namespace Application.Contracts;

public interface IViewService
{
    UnitResult<Error> ShowStats(StatsDto stats);

    Task<T> ShowProgressBarAsync<T>(string progressDescription, Func<IProgressTracker, Task<T>> whileProgressRunsAsync);

    void ShowError(Error error);
}