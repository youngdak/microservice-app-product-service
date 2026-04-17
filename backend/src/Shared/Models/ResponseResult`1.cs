namespace Shared.Models;

public class ResponseResult<T>
{
    private readonly ResponseResultStatus _status;

    public string Message { get; }

    public T Value { get; }

    public bool IsSuccess => _status == ResponseResultStatus.Success;

    public bool IsFailure => _status == ResponseResultStatus.Fail;

    protected internal ResponseResult(ResponseResultStatus status, string message, T value)
    {
        _status = status;
        Message = message;
        Value = value;
    }
}