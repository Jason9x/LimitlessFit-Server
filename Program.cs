using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LimitlessFit.Data;
using LimitlessFit.Interfaces;
using LimitlessFit.Middleware;
using LimitlessFit.Services;
using LimitlessFit.Services.Hubs;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

ConfigureServices(builder.Services);
ConfigureSwagger(builder.Services);
ConfigureDatabase(builder.Services);
ConfigureAuthentication(builder.Services);
ConfigureSignalR(builder.Services);

var app = builder.Build();

await SeedDatabase(app);

ConfigureAppPipeline(app);

app.Run();

return;

void ConfigureServices(IServiceCollection services)
{
    services.AddControllers()
        .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

    services.AddHttpContextAccessor();

    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IItemService, ItemService>();
    services.AddScoped<IOrderService, OrderService>();
    services.AddScoped<INotificationService, NotificationService>();
    services.AddScoped<IUserService, UserService>();
    
    services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
        {
            policyBuilder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}

void ConfigureSwagger(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

void ConfigureDatabase(IServiceCollection services)
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
    var dbUser = Environment.GetEnvironmentVariable("DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");

    var connectionString = $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPassword}";

    services.AddDbContextPool<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .UseSnakeCaseNamingConvention());
}

void ConfigureAuthentication(IServiceCollection services)
{
    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
    var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
    var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

    services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? string.Empty))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    
                    if (!string.IsNullOrEmpty(accessToken))
                        context.Token = accessToken;

                    return Task.CompletedTask;
                }
            };
        });
}

void ConfigureSignalR(IServiceCollection services)
{
    services.AddSignalR(options => { options.EnableDetailedErrors = true; });
}

async Task SeedDatabase(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var environment = services.GetRequiredService<IHostEnvironment>();

    await DatabaseSeeder.SeedDatabaseAsync(context, environment);
}

void ConfigureAppPipeline(WebApplication webApplication)
{
    if (webApplication.Environment.IsDevelopment())
    {
        webApplication.UseSwagger();
        webApplication.UseSwaggerUI();
    }

    webApplication.UseCors("AllowSpecificOrigins");
    webApplication.UseHttpsRedirection();
    webApplication.UseAuthentication();
    app.UseMiddleware<DynamicRoleMiddleware>();
    webApplication.UseAuthorization();
    webApplication.MapControllers();

    webApplication.MapHub<OrderUpdateHub>("/orderUpdateHub");
    webApplication.MapHub<NotificationHub>("/notificationHub");
    webApplication.MapHub<UserHub>("/userHub");
}