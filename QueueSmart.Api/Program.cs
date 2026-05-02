using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Data;
using QueueSmart.Api.Services;

// builder is used to build the application
var builder = WebApplication.CreateBuilder(args);

// we connecting to the database with this one
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// controllers with json enum serialization as camelcase strings
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
    });

builder.Services.AddOpenApi();

// services and stores
builder.Services.AddScoped<IServiceStore, ServiceStore>();
builder.Services.AddScoped<IHistoryStore, HistoryStore>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserStore, DbUserStore>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddScoped<IQueueStore, QueueStore>();

// cors policy
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

// ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

// if the application is in development mode, map the OpenApi
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// use the cors policy
app.UseCors("AllowFrontend");
// map the controllers
app.MapControllers();

// run the application
app.Run();
