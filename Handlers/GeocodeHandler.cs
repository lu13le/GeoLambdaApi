using System.Text.Json;
using GeoLambdaApi.Handlers.Interfaces;
using GeoLambdaApi.Models;
using Microsoft.Extensions.Options;
using GeoLambdaApi.Models.GoogleApi;
using GeoLambdaApi.Models.Options;
using GeoLambdaApi.Models.Requests;
using GeoLambdaApi.Models.Responses;
using GeoLambdaApi.Repositories.Interfaces;
using GeoLambdaApi.Validations.Interfaces;
using GeoLambdaApi.Mappers;
using GeoLambdaApi.Helpers;
using static GeoLambdaApi.Models.Constants.GeocodeHandlerConstants;

namespace GeoLambdaApi.Handlers;

public class GeocodeHandler : IGeocodeHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleOptions _googleOptions;
    private readonly IValidationHelper _validator;
    private readonly ILogger<GeocodeHandler> _logger;
    private readonly IDocumentStoreRepository _documentStore;

    public GeocodeHandler(
        IHttpClientFactory httpClientFactory,
        IOptions<GoogleOptions> googleOptions,
        IValidationHelper validator,
        ILogger<GeocodeHandler> logger, IDocumentStoreRepository documentStore)
    {
        _httpClientFactory = httpClientFactory;
        _googleOptions = googleOptions.Value;
        _validator = validator;
        _logger = logger;
        _documentStore = documentStore;
    }

    public async Task<Result<GeocodeResponse>> HandleAsync(GeocodeRequest request, CancellationToken ct = default)
    {
        var validationFailure = ValidateRequest(request);
        if (validationFailure is not null)
        {
            return validationFailure;
        }
        var apiKeyCheck = EnsureApiKey();
        if (apiKeyCheck is not null)
        {
            return apiKeyCheck;
        }

        var address = GeocodeHelper.NormalizeAddress(request.Address!);

        // 1) Try to serve from cache if not older than 30 days
        var cached = await _documentStore.GetLatestByAddressAsync(address);
        if (cached is not null)
        {
            var maxPeriod = TimeSpan.FromDays(30);
            var daysCached = DateTimeOffset.UtcNow - cached.Timestamp;
            if (daysCached <= maxPeriod)
            {
                var cachedResponse = GeocodeMapper.MapToResponse(cached);
                return Result<GeocodeResponse>.Success(cachedResponse);
            }
        }

        // 2) Fallback to Google when no cache or cache is older than 30 days
        var url = GeocodeHelper.BuildGeocodeUrl(address, _googleOptions.ApiKey!);

        var fetchResult = await FetchGoogleGeocodeAsync(url, ct);
        if (!fetchResult.IsSuccess)
        {
            return Result<GeocodeResponse>.Failure(ErrorCodeUpstream, ErrorMessageUpstreamFailed);
        }

        var root = fetchResult.Value!;
        var notFound = GeocodeHelper.HasNoResults(root);
        if (notFound)
        {
            return Result<GeocodeResponse>.Failure(ErrorCodeNotFound, ErrorMessageNotFound);
        }

        if (!GeocodeHelper.IsOkStatus(root))
        {
            _logger.LogWarning(LogNonOkStatus, root.Status);
            return Result<GeocodeResponse>.Failure(ErrorCodeUpstream, ErrorMessageUpstreamFailed);
        }

        var response = GeocodeMapper.MapToResponse(root, address);
        await _documentStore.CreateAsync(GeocodeMapper.MapToDatabaseEntry(response));
        return Result<GeocodeResponse>.Success(response);
    }

    private Result<GeocodeResponse>? ValidateRequest(GeocodeRequest request)
    {
        var errors = _validator.Validate(request);
        if (errors.Count == 0) return null;

        var problem = errors.ToDictionary(e => e, _ => StringArray);
        return Result<GeocodeResponse>.Failure(ErrorCodeValidation, ErrorMessageValidationFailed, problem);
    }

    private Result<GeocodeResponse>? EnsureApiKey()
    {
        var apiKey = _googleOptions.ApiKey;
        return !string.IsNullOrWhiteSpace(apiKey) ? null : Result<GeocodeResponse>.Failure(ErrorCodeInternal, ErrorMessageApiKeyMissing);
    }

    private async Task<Result<GoogleGeocodeResponse>> FetchGoogleGeocodeAsync(string url, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient(ClientNameGoogleGeocode);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

        try
        {
            using var response = await client.SendAsync(httpRequest, ct);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            var root = await JsonSerializer.DeserializeAsync<GoogleGeocodeResponse>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                ct) ?? new GoogleGeocodeResponse(null, null);

            return Result<GoogleGeocodeResponse>.Success(root);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogRequestFailed);
            return Result<GoogleGeocodeResponse>.Failure(ErrorCodeUpstream, ErrorMessageUpstreamFailed);
        }
    }
}
