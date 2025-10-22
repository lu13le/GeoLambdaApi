namespace GeoLambdaApi.Models.Constants;

public static class GeocodeHandlerConstants
{
    public const string ClientNameGoogleGeocode = "google-geocode";
    public const string GoogleGeocodeUrlFormat = "https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}";
    public const string StatusOk = "Ok";
    public const string StatusZeroResults = "ZERO_RESULTS";
    public const string ErrorCodeValidation = "Validation";
    public const string ErrorCodeInternal = "Internal";
    public const string ErrorCodeUpstream = "Upstream";
    public const string ErrorCodeNotFound = "NotFound";
    public const string ErrorMessageValidationFailed = "Validation Failed.";
    public const string ErrorMessageApiKeyMissing = "Google API Key is not configured. Set Google API Key in your appsettings.json file.";
    public const string ErrorMessageUpstreamFailed = "Failed to fetch geocoding data.";
    public const string ErrorMessageNotFound = "No geocoding data found for the given address.";
    public const string LogNonOkStatus = "Google Geocode returned non-Ok status: {Status}";
    public const string LogRequestFailed = "Google Geocode request failed.";
    public static readonly string[] StringArray = ["Invalid"];
}