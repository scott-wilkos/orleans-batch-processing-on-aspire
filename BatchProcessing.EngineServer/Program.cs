using BatchProcessing.Grains;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddBatchProcessingEngineApplication(builder.Configuration);

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(orleansBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        orleansBuilder.ConfigureEndpoints(Random.Shared.Next(10_000, 50_000), Random.Shared.Next(10_000, 50_000));
        orleansBuilder.UseDashboard(options => { options.HostSelf = true;
            options.Port = Random.Shared.Next(10_000, 50_000);
        });
    }
});

var host = builder.Build();
host.Map("/dashboard", x => x.UseOrleansDashboard());

host.Run();

