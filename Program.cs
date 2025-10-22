using Amazon;
using Amazon.DynamoDBv2;
using GeoLambdaApi.Handlers.Interfaces;
using GeoLambdaApi.Models.Options;
using GeoLambdaApi.Repositories;
using GeoLambdaApi.Repositories.Interfaces;
using GeoLambdaApi.Validations;
using GeoLambdaApi.Validations.Interfaces;
using GeoLambdaApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services
var config = builder.Configuration;

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<GoogleOptions>(builder.Configuration.GetSection("Google"));
builder.Services.AddScoped<IValidationHelper, ValidationHelper>();
builder.Services.AddScoped<IGeocodeHandler, GeocodeHandler>();
builder.Services.AddHttpClient("google-geocode", client =>
{
    client.Timeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(RegionEndpoint.EUCentral1));
builder.Services.AddSingleton<IDocumentStoreRepository>(provider =>
    new DocumentStoreRepository(provider.GetRequiredService<IAmazonDynamoDB>(),
        config.GetValue<string>("Database:TableName") ?? string.Empty));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

