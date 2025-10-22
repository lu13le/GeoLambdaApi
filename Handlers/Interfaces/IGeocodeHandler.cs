using System.Threading;
using System.Threading.Tasks;
using GeoLambdaApi.Models;
using GeoLambdaApi.Models.Requests;
using GeoLambdaApi.Models.Responses;

namespace GeoLambdaApi.Handlers.Interfaces;

public interface IGeocodeHandler
{
    Task<Result<GeocodeResponse>> HandleAsync(GeocodeRequest request, CancellationToken cancellationToken);
}