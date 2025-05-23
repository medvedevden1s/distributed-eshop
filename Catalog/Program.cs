using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAiService>();
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

// Register Ollama-based chat & embedding
builder.AddOllamaSharpChatClient("ollama-llama3-2");
builder.AddOllamaSharpEmbeddingGenerator("ollama-all-minilm");

// Register an in-memory vector store
builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
    app.UseMigration();  // For prod we should have something like SQL Server Agent jobs, Flyway, Liquibase, etc. 

app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();
