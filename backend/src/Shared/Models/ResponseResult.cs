namespace Shared.Models;

public class ResponseResult : ResponseResult<string>
{
    protected internal ResponseResult(ResponseResultStatus status, string message, string value)
        : base(status, message, value)
    {
    }

    public static ResponseResult Ok()
    {
        return new ResponseResult(ResponseResultStatus.Success, null, null);
    }

    public static ResponseResult Fail(string message)
    {
        return new ResponseResult(ResponseResultStatus.Fail, message, null);
    }

    public static ResponseResult<T> Ok<T>(T data)
    {
        return new ResponseResult<T>(ResponseResultStatus.Success, null, data);
    }

    public static ResponseResult<T> Fail<T>(string message)
    {
        return new ResponseResult<T>(ResponseResultStatus.Fail, message, default(T));
    }
}