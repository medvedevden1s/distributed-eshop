var builder = DistributedApplication.CreateBuilder(args);

// Backing services
var postgres = builder.AddPostgres("postgress") // spin up pg container
    .WithPgAdmin() // Optional admin panel
    .WithDataVolume() // add volume to docker container 
    .WithLifetime(ContainerLifetime.Persistent); // this allows to run the container when server is stopped

var catalogDb = postgres.AddDatabase("catalogdb");

var cache = builder
    .AddRedis("cache")
    .WithRedisInsight() // Optional Monitoring and Visualization redis keys
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Projects
builder.AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WaitFor(cache);

builder.Build().Run();
