using System.Text.Json.Serialization;

namespace GeoLambdaApi.Models.Data;

public class GeocodeEntry
{
    [JsonPropertyName("pk")]
    public string Pk => Id;
    [JsonPropertyName("sk")]
    public string Sk => Id;
    public string Id { get; init; }  = string.Empty;
    public string Address { get; init; }  = string.Empty;
    public double Longitude { get; init; }
    public double Latitude { get; init; }  
    public DateTimeOffset Timestamp { get; init; }
}