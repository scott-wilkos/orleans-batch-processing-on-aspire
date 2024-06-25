using BatchProcessing.ApiService.Grains;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(orleansBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        orleansBuilder.ConfigureEndpoints(Random.Shared.Next(10_000, 50_000), Random.Shared.Next(10_000, 50_000));
    }
});

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapPost("/batchProcessing/{records}", async (IClusterClient client, int records) =>
{
    var grain = client.GetGrain<IEngineGrain>(Guid.NewGuid());

    await grain.RunAnalysis(records);

    return grain.GetPrimaryKey();
});

app.MapGet("/batchProcessing/{id}/status", async (IClusterClient client, Guid id) =>
{
    var grain = client.GetGrain<IEngineGrain>(id);
    var status = await grain.GetStatus();
    return status;
});

app.MapDefaultEndpoints();

app.Run();