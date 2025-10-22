using System.Collections.Generic;
using GeoLambdaApi.Models.Requests;

namespace GeoLambdaApi.Validations.Interfaces;

public interface IValidationHelper
{
    IReadOnlyList<string> Validate(GeocodeRequest? request);
}