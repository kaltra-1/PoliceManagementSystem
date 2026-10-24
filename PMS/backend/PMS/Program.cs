using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PMS.Data;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Police Management System API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Enter 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "PMS_DefaultSecretKey_ChangeInProduction_2024!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "PMS",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "PMS",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// CORS — allow frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Apply pending EF Core migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // Try to apply pending migrations. If the model has classes that aren't
        // in any migration (like newer Department/Setting/Notification tables),
        // those must be created via database_update.sql — the script is the
        // single source of truth for schema in this project.
        db.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");

        // Verify the new tables exist; if not, hint to run the SQL script
        try
        {
            var canQuery = db.Departments.Any() || true;
            logger.LogInformation("New tables (Departments, Settings, Notifications) are present.");
        }
        catch
        {
            logger.LogWarning(
                "New tables (Departments, Settings, Notifications) not found. " +
                "Please run database_update.sql first.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Migration failed. Make sure the database_update.sql has been run first, " +
            "or that SQL Server is reachable at the configured connection string.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
