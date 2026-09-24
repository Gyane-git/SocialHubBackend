using SocialHub.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register application services, EF Core DbContext, Swagger, CORS, and Controllers
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Global Exception Handler Middleware
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SocialHub API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors(ServiceCollectionExtensions.CorsPolicyName);

app.UseAuthorization();

app.MapControllers();

// Apply EF Core migrations and deterministic seed data
await app.ApplyMigrationsAndSeedAsync();

app.Run();
