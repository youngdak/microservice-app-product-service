using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace ProductService.Api.Endpoints;

internal static class ApiResponse
{
    internal static IResult Ok()
    {
        return Results.Ok(ApiResponseResult<string>.Ok());
    }

    internal static IResult Ok<T>(T data)
    {
        return Results.Ok(ApiResponseResult<T>.Ok(data));
    }

    internal static IResult Error(string error)
    {
        return Results.BadRequest(ApiResponseResult<string>.Fail(error));
    }

    internal static IResult FromResult(ResponseResult outcome)
    {
        return outcome.IsSuccess ? Ok() : Error(outcome.Message);
    }

    internal static IResult FromResult<T>(ResponseResult result, T value)
    {
        if (result.IsSuccess)
        {
            return Ok(value);
        }

        if (result.IsFailure)
        {
            return Error(result.Message);
        }

        return Ok();
    }

    internal static IResult FromResult<T>(ResponseResult<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        if (result.IsFailure)
        {
            return Error(result.Message);
        }

        return Ok();
    }

    internal static IResult FromResult<T>(ResponseResult<T> result, T value)
    {
        if (result.IsSuccess)
        {
            return Ok(value);
        }

        if (result.IsFailure)
        {
            return Error(result.Message);
        }

        return Ok();
    }

    internal static IResult FromResult<T>(ResponseResult result, Func<T> func)
    {
        if (result.IsSuccess)
        {
            return Ok(func.Invoke());
        }

        if (result.IsFailure)
        {
            return Error(result.Message);
        }

        return Ok();
    }

    internal static IResult FromResult<T>(ResponseResult<T> result, Func<T> func)
    {
        if (result.IsSuccess)
        {
            return Ok(func.Invoke());
        }

        if (result.IsFailure)
        {
            return Error(result.Message);
        }

        return Ok();
    }
}

internal class ApiResponseResult<T>
{
    public string Status { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; }

    private ApiResponseResult(string status, string? message, T? data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public static ApiResponseResult<T> Ok(T? data)
    {
        return new ApiResponseResult<T>(ResponseResultStatus.Success.ToString(), null, data);
    }

    public static ApiResponseResult<T> Ok()
    {
        return new ApiResponseResult<T>(ResponseResultStatus.Success.ToString(), null, default(T));
    }

    public static ApiResponseResult<T> Fail(string message)
    {
        return new ApiResponseResult<T>(ResponseResultStatus.Fail.ToString(), message, default(T));
    }
}
