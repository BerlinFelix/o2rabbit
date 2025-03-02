using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.Extensions.Options;
using o2rabbit.Api.Options;
using o2rabbit.Api.Options.Connection;
using o2rabbit.Api.Options.Cors;
using o2rabbit.BizLog.Extensions;

namespace o2rabbit.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOptions()
            .Configure<ApiCorsOptions>(builder.Configuration.GetRequiredSection("CorsOptions"))
            .AddOptionsWithValidateOnStart<ConnectionStringOptions>()
            .BindConfiguration("ConnectionStringOptions");

        builder.Services.ConfigureOptions<ConnectionStringOptionsConfigurator>();

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(_FluentValidationRegistrationHook),
            includeInternalTypes: true);

        builder.Services.AddCors(options =>
            {
                var corsOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApiCorsOptions>>()
                    .Value;
                options.AddPolicy(corsOptions.PolicyName,
                    b =>
                    {
                        b.WithOrigins(corsOptions.Origins);
                        if (corsOptions.Headers.Length > 0)
                            b.WithHeaders(corsOptions.Headers);
                        else
                            b.AllowAnyHeader();
                        if (corsOptions.Methods.Length > 0)
                            b.WithMethods(corsOptions.Methods);
                        else
                            b.AllowAnyMethod();
                    });
            })
            .AddBizLog((o, sp) =>
            {
                var connectionStringOptions = sp.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
                o.ConnectionStringMainDb = connectionStringOptions.ConnectionStringMainDb;
            })
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

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