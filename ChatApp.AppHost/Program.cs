var builder = DistributedApplication.CreateBuilder(args);

// var cache = builder.AddRedis("cache");

builder.AddProject<Projects.ChatApp_Application>("Application")
    .WithExternalHttpEndpoints();
    // .WithReference(cache)
    // .WaitFor(cache)

builder.Build().Run();
