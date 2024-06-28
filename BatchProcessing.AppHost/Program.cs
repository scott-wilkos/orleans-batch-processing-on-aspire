var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var orleans = builder.AddOrleans("orleans-engine")
    .WithClustering(redis);

builder.AddProject<Projects.BatchProcessing_Dashboard>("dashboard")
    .WithReference(redis)
    .WithReference(orleans);

var engine = builder.AddProject<Projects.BatchProcessing_Engine>("engine")
    .WithReference(redis)
    .WithReference(orleans)
    .WithReplicas(3);

var apiService = builder.AddProject<Projects.BatchProcessing_ApiService>("api-service")
    .WithReference(redis)
    .WithReference(orleans.AsClient());

builder.AddProject<Projects.BatchProcessing_Web>("web-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();