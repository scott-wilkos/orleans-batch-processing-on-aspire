var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");

builder.UseOrleans(orleansBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        orleansBuilder.ConfigureEndpoints(Random.Shared.Next(10_000, 50_000), Random.Shared.Next(10_000, 50_000));
        orleansBuilder.UseDashboard(options => { options.HostSelf = true; });
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.Map("/dashboard", x => x.UseOrleansDashboard());

app.Run();