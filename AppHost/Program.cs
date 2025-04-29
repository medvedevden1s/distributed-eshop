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

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitmq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitmq);

var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(rabbitmq)
    .WithReference(keycloak)
    .WaitFor(cache)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

builder.Build().Run();
