using System.Collections.Generic;

namespace GeoLambdaApi.Models;

public record Error(string Code, string Message, IDictionary<string, string[]>? Validation = null);

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(string code, string message,
        IDictionary<string, string[]>? validationErrors = null) =>
        new(new Error(code, message, validationErrors));
}