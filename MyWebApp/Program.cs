using FluentValidation;
using MyWebApp.Core.Interfaces;
using MyWebApp.Infrastructure.Services;

namespace Azure_Project_001_MyWebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register exception handler
        builder.Services.AddExceptionHandler<Middleware.GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Register FluentValidation validators
        builder.Services.AddValidatorsFromAssemblyContaining<MyWebApp.Core.Validators.GetWeatherForecastRequestValidator>();

        // Register application services
        builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Add exception handler middleware
        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
