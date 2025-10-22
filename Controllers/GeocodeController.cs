using System.Threading;
using System.Threading.Tasks;
using GeoLambdaApi.Extensions;
using GeoLambdaApi.Handlers.Interfaces;
using GeoLambdaApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GeoLambdaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeocodeController(IGeocodeHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Geocode([FromQuery] GeocodeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(request, cancellationToken);
        return result.ToActionResult(this);
    }
}