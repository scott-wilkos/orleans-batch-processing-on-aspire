var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var postgres = builder.AddPostgres("postgres");
var postgresDb = postgres.AddDatabase("postgresDb");

var orleans = builder.AddOrleans("orleans-engine")
    .WithClustering(redis);

builder.AddProject<Projects.BatchProcessing_Dashboard>("dashboard")
    .WithReference(redis)
    .WithReference(orleans);

var engine = builder.AddProject<Projects.BatchProcessing_EngineServer>("engine")
    .WithReference(redis)
    .WithReference(orleans)
    .WithReference(postgresDb)
    .WithReplicas(3);

var apiService = builder.AddProject<Projects.BatchProcessing_ApiService>("api-service")
    .WithReference(redis)
    .WithReference(orleans.AsClient());

builder.AddProject<Projects.BatchProcessing_Web>("web-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();