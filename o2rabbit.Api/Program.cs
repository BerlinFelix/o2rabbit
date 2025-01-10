using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using o2rabbit.Api.Options;
using o2rabbit.BizLog.Extensions;

namespace o2rabbit.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("Default") ??
                               throw new NullReferenceException("Default connection string");
        // Add services to the container.
        builder.Services.AddBizLog((o, sp) => { o.ConnectionString = connectionString; })
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(_FluentValidationRegistrationHook),
            includeInternalTypes: true);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy",
                b => { b.WithOrigins("http://localhost:5173", "http://localhost:5173/*").AllowAnyHeader(); });
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Logging.AddConsole(options => { options.LogToStandardErrorThreshold = LogLevel.Information; });

        var app = builder.Build();

        app.UseCors("DefaultPolicy");
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}