using TaskManagementBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagementBackend.Seed;



var builder = WebApplication.CreateBuilder(args);

var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST");
var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT");
var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");
var dbUser = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
var dbPass = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass}";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var corsUrls = Environment.GetEnvironmentVariable("CORS_URL")?.Split(",") ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(corsUrls)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var port = Environment.GetEnvironmentVariable("PORT") ?? "5297";
app.Urls.Add($"http://*:{port}");
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    DbSeeder.SeedRootUser(scope.ServiceProvider);
}

app.Run();
