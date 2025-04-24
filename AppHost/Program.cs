var builder = DistributedApplication.CreateBuilder(args);

// Backing services
var postgres = builder.AddPostgres("postgress") // spin up pg container
    .WithPgAdmin() // optional
    .WithDataVolume() // add volume to docker container 
    .WithLifetime(ContainerLifetime.Persistent); // this allows to run the container when server is stopped

var catalogDb = postgres.AddDatabase("catalogdb");

// Projects
builder.AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

builder.Build().Run();
