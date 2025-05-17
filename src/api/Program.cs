using MinecraftDataWrapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Register our Minecraft data service
builder.Services.AddSingleton<MinecraftDataApi.Data.IMinecraftDataService, MinecraftDataApi.Data.JsonFileMinecraftDataService>();

// Register MinecraftDataWrapper service
string minecraftDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "external", "minecraft-data");
builder.Services.AddMinecraftData(minecraftDataPath, "1.20.2");

// Register our adapter that integrates MinecraftDataWrapper with our existing API
builder.Services.AddScoped<MinecraftDataApi.Data.MinecraftDataWrapperAdapter>();

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add configuration for data paths
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.MapControllers();

app.Run();
