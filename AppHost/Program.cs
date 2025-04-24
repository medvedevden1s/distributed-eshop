var builder = DistributedApplication.CreateBuilder(args);

// add procjects and cloud native backing services here

builder.AddProject<Projects.Catalog>("catalog");

// add procjects and cloud native backing services here

builder.Build().Run();
