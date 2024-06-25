var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var orleans = builder.AddOrleans("orleans-engine")
    .WithClustering(redis);

var apiService = builder.AddProject<Projects.BatchProcessing_ApiService>("apiservice")
    .WithReference(redis)
    .WithReference(orleans);

builder.AddProject<Projects.BatchProcessing_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
