using OrleansEngine;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(orleansBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        orleansBuilder.ConfigureEndpoints(Random.Shared.Next(10_000, 50_000), Random.Shared.Next(10_000, 50_000));
    }
});

var host = builder.Build();
host.Run();
