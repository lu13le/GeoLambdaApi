using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeoLambdaApi.Models.Requests;
using GeoLambdaApi.Validations.Interfaces;

namespace GeoLambdaApi.Validations;

public partial class ValidationHelper : IValidationHelper
{
    private static readonly Regex AllowedChars = MyRegex();
    public IReadOnlyList<string> Validate(GeocodeRequest? request)
    {
        var errors = new List<string>();
        if (request is null)
        {
            errors.Add("Request body is required.");
            return errors;
        }

        if (string.IsNullOrEmpty(request.Address))
        {
            errors.Add("Address is required.");
        }
        else
        {
            var trimmed = request.Address.Trim();
            switch (trimmed.Length)
            {
                case<3:
                    errors.Add("Address must have 3 characters at least.");
                    break;
                case>150:
                    errors.Add("Address can have 150 characters maximum.");
                    break;
            }

            if (!AllowedChars.IsMatch(trimmed))
            {
                errors.Add("Address contains invalid characters.");
            }
        }

        return errors;
    }

    [GeneratedRegex("^[\x20-\x7E]+$")]
    private static partial Regex MyRegex();
}