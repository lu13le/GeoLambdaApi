using GeoLambdaApi.Models.Data;

namespace GeoLambdaApi.Repositories.Interfaces;

public interface IDocumentStoreRepository
{
    Task<bool> CreateAsync(GeocodeEntry entry);
    Task<GeocodeEntry?> GetLatestByAddressAsync(string address);
}