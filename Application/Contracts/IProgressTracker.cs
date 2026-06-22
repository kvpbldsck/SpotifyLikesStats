namespace Application.Contracts;

public interface IProgressTracker
{
    void SetGoal(int goal);
    void IncreaseProgress(int value);
    void SetDescription(string description);
}