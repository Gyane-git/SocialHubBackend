using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Services;

namespace SocialHub.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public const string CorsPolicyName = "FrontendCorsPolicy";

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. DbContext Configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        services.AddDbContext<SocialHubDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        });

        // 2. Dependency Injection for Business Services
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISocialAccountService, SocialAccountService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IHashtagService, HashtagService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // 3. CORS Configuration for Next.js frontend
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // 4. Controllers with JSON options and custom invalid model state response
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors.Select(err =>
                            string.IsNullOrEmpty(err.ErrorMessage) ? "Invalid input value." : err.ErrorMessage))
                        .ToList();

                    var response = ApiResponse.Fail("Validation failed.", errors);
                    return new BadRequestObjectResult(response);
                };
            });

        // 5. Swagger / OpenAPI Configuration
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SocialHub API",
                Version = "v1",
                Description = "Social Media Marketing Management Platform API - Phase 1 Backend Services."
            });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
