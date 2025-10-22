using System.Text.Encodings.Web;
using GeoLambdaApi.Models.GoogleApi;
using static GeoLambdaApi.Models.Constants.GeocodeHandlerConstants;

namespace GeoLambdaApi.Helpers;

public static class GeocodeHelper
{
    public static string NormalizeAddress(string address) => address.Trim();

    public static string BuildGeocodeUrl(string address, string apiKey)
    {
        var addressEncoded = UrlEncoder.Default.Encode(address);
        return string.Format(GoogleGeocodeUrlFormat, addressEncoded, apiKey);
    }

    public static bool HasNoResults(GoogleGeocodeResponse root) =>
        root.Status == StatusZeroResults || root.Results is null || root.Results.Length == 0;

    public static bool IsOkStatus(GoogleGeocodeResponse root) =>
        string.Equals(root.Status, StatusOk, StringComparison.CurrentCultureIgnoreCase);
}