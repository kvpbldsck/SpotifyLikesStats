namespace Application.Models;

public sealed record Error(
    ErrorCode Code,
    string Message)
{
    public static Error Failure(string message)
        => new(ErrorCode.Failure, message);

    public static Error Authorization(string message)
        => new(ErrorCode.AuthorizationFailed, message);

    public static Error Request(string message)
        => new(ErrorCode.RequestFailed, message);

    public override string ToString() => Message;
}

public enum ErrorCode
{
    Failure,
    AuthorizationFailed,
    RequestFailed,
}