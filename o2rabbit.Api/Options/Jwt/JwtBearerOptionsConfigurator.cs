using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace o2rabbit.Api.Options.Jwt;

internal class JwtBearerOptionsConfigurator : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public JwtBearerOptionsConfigurator(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _configuration = configuration;
    }

    public void Configure(JwtBearerOptions options)
    {
        _configuration.GetRequiredSection("JwtBearer").Bind(options);
        options.TokenValidationParameters.ValidIssuer = options.Authority;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}