using System;

namespace GeoLambdaApi.Models.Responses;

public record GeocodeResponse(string Id, string Address, double Longitude, double Latitude, DateTimeOffset Timestamp);