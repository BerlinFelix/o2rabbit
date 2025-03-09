using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;
using o2rabbit.Api.Options;
using o2rabbit.Api.Options.Connection;
using o2rabbit.Api.Options.Cors;
using o2rabbit.Api.Options.Jwt;
using o2rabbit.BizLog.Extensions;

namespace o2rabbit.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

#if DEBUG
        builder.Configuration.AddUserSecrets(typeof(Program).Assembly); // Add user secrets (for development)
#endif

        builder.Services.AddOptions()
            .Configure<ApiCorsOptions>(builder.Configuration.GetRequiredSection("CorsOptions"))
            .AddOptionsWithValidateOnStart<ConnectionStringOptions>()
            .BindConfiguration("ConnectionStringOptions");
        builder.Services.ConfigureOptions<ConnectionStringOptionsConfigurator>();
        builder.Services.ConfigureOptions<JwtBearerOptionsConfigurator>();

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(_FluentValidationRegistrationHook),
                includeInternalTypes: true)
            .AddCors(options =>
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

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });
        builder.Logging.ClearProviders().AddConsole().SetMinimumLevel(LogLevel.Information);

        var app = builder.Build();

        app.UseCors("DefaultPolicy");
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // app.UseAuthentication()
        //     .UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}