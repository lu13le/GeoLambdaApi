using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using GeoLambdaApi.Models.Data;
using GeoLambdaApi.Repositories.Interfaces;
using Document = Amazon.DynamoDBv2.DocumentModel.Document;

namespace GeoLambdaApi.Repositories;

public class DocumentStoreRepository : IDocumentStoreRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;

    public DocumentStoreRepository(IAmazonDynamoDB dynamoDb, string tableName)
    {
        _dynamoDb = dynamoDb;
        _tableName = tableName;
    }

    public async Task<bool> CreateAsync(GeocodeEntry entry)
    {
        var entryAsJson = JsonSerializer.Serialize(entry);
        var itemAsDocument = Document.FromJson(entryAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();
        var createItemRequest = new PutItemRequest()
        {
            TableName = _tableName,
            Item = itemAsAttributes
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<GeocodeEntry?> GetLatestByAddressAsync(string address)
    {
        var response = await _dynamoDb.ScanAsync(BuildAddressScanRequest(address));
        if (response.Items == null || response.Items.Count == 0)
        {
            return null;
        }

        var entries = response.Items
            .Select(TryDeserializeEntry)
            .Where(e => e != null)
            .Select(e => e!);

        return SelectLatest(entries);
    }

    private ScanRequest BuildAddressScanRequest(string address)
    {
        return new ScanRequest
        {
            TableName = _tableName,
            FilterExpression = "Address = :address",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":address", new AttributeValue { S = address } }
            }
        };
    }

    private static GeocodeEntry? TryDeserializeEntry(Dictionary<string, AttributeValue> item)
    {
        try
        {
            var doc = Document.FromAttributeMap(item);
            var json = doc.ToJson();
            return JsonSerializer.Deserialize<GeocodeEntry>(json);
        }
        catch
        {
            // ignore deserialization issues for individual items
            return null;
        }
    }

    private static GeocodeEntry? SelectLatest(IEnumerable<GeocodeEntry> entries)
    {
        // returns null if entries is empty
        return entries.MaxBy(e => e.Timestamp);
    }
}