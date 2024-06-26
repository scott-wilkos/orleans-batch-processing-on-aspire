var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var orleans = builder.AddOrleans("orleans-engine")
    .WithClustering(redis);

builder.AddProject<Projects.BatchProcessing_Dashboard>("batchprocessing-dashboard")
    .WithReference(redis)
    .WithReference(orleans);

var engine = builder.AddProject<Projects.BatchProcessing_Engine>("batchprocessing-engine")
    .WithReference(redis)
    .WithReference(orleans)
    .WithReplicas(3);

var apiService = builder.AddProject<Projects.BatchProcessing_ApiService>("apiservice")
    .WithReference(redis)
    .WithReference(orleans.AsClient());

builder.AddProject<Projects.BatchProcessing_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
