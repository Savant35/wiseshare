using WiseShare.Api;
using WiseShare.Application;
using WiseShare.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container
    builder.Services.AddControllers(); // Enable controller support
    builder.Services.AddEndpointsApiExplorer(); // Enable Swagger/OpenAPI support
    builder.Services.AddSwaggerGen(); // Configure Swagger

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    builder.Services
           .AddPresentation()  // Add Presentation layer services
           .AddApplication()   // Add Application layer services
           .AddInfrastructure(builder.Configuration); // Add Infrastructure layer services
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(); // Enable Swagger in Development
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection(); // Redirect HTTP to HTTPS

    app.UseCors("AllowReactApp");
    app.MapControllers();      // Map all controller routes

    app.Run();                 // Start the application
}
