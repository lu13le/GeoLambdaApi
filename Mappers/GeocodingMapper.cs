using GeoLambdaApi.Models.Data;
using GeoLambdaApi.Models.GoogleApi;
using GeoLambdaApi.Models.Responses;

namespace GeoLambdaApi.Mappers;

public static class GeocodeMapper
{
    public static GeocodeResponse MapToResponse(GoogleGeocodeResponse root, string address)
    {
        var first = root.Results![0];
        return new GeocodeResponse(
            Guid.NewGuid().ToString("N"),
            first.FormattedAddress ?? address,
            first.Geometry?.Location?.Lat ?? 0,
            first.Geometry?.Location?.Lng ?? 0,
            DateTimeOffset.UtcNow
        );
    }

    public static GeocodeResponse MapToResponse(GeocodeEntry entry)
    {
        return new GeocodeResponse(
            entry.Id,
            entry.Address,
            entry.Latitude,
            entry.Longitude,
            entry.Timestamp
        );
    }

    public static GeocodeEntry MapToDatabaseEntry(GeocodeResponse response)
    {
        return new GeocodeEntry()
        {
            Id = response.Id,
            Address = response.Address,
            Latitude = response.Latitude,
            Longitude = response.Longitude,
            Timestamp = response.Timestamp
        };
    }
}