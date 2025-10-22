using System.Collections.Generic;
using GeoLambdaApi.Models;
using Microsoft.AspNetCore.Mvc;
using GeoLambdaApi.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace GeoLambdaApi.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result<GeocodeResponse> result, ControllerBase controller)
    {
        if (result is { IsSuccess: true, Value: not null })
        {
            var geocodeResponse = result.Value;
            return controller.Ok(geocodeResponse);
        }

        var error = result.Error ?? new Error("Internal", "Unknown Error");

        return error.Code switch
        {
            "Validation" => controller.ValidationProblem(
                new ValidationProblemDetails(error.Validation ?? new Dictionary<string, string[]>())
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                }),
            "BadRequest" => controller.Problem(title: "Bad Request", detail: error.Message,
                statusCode: StatusCodes.Status400BadRequest),
            "NotFound" => controller.Problem(title: "Not Found", detail: error.Message,
                statusCode: StatusCodes.Status404NotFound),
            "Upstream" => controller.Problem(title: "Bad Gateway", detail: error.Message,
                statusCode: StatusCodes.Status502BadGateway),
            _ => controller.Problem(title: "Internal Server Error", detail: error.Message,
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}