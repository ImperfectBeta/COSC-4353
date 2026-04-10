using System.Text.Json.Serialization;
using QueueSmart.Api.Services;
using Microsoft.EntityFrameworkCore;
using QueueSmart.Api;

// builder is used to build the application
var builder = WebApplication.CreateBuilder(args);

// we connecting to the database with this one
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// controllers w/ JSON enum serialization as camelCase strings
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
    });

builder.Services.AddOpenApi();

// in-memory stores as singletons (a singleton is a class that can only have one instance)
builder.Services.AddSingleton<IServiceStore, ServiceStore>();
builder.Services.AddSingleton<IHistoryStore, HistoryStore>();
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();
builder.Services.AddSingleton<IAuthService, AuthService>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// app is the application
var app = builder.Build();

// if the application is in development mode, map the OpenApi
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// use the CORS policy
app.UseCors("AllowFrontend");
// map the controllers
app.MapControllers();

// run the application
app.Run();
