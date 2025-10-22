using System.Text.Json.Serialization;

namespace GeoLambdaApi.Models.GoogleApi;

public record GeocodeLocation(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng);

public record GeocodeGeometry(
    [property: JsonPropertyName("location")]
    GeocodeLocation? Location);

public record GeocodeItem(
    [property: JsonPropertyName("formatted_address")]
    string? FormattedAddress,
    [property: JsonPropertyName("geometry")]
    GeocodeGeometry? Geometry);

public record GoogleGeocodeResponse(
    [property: JsonPropertyName("status")] string? Status,
    [property: JsonPropertyName("results")] GeocodeItem []? Results);