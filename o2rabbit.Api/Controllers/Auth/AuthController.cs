using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using o2rabbit.Api.Models;
using o2rabbit.Api.Parameters;

namespace o2rabbit.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> GetToken([FromBody] LoginParameters loginParameters,
        CancellationToken cancelationToken = default)
    {
        //TODO
        if (loginParameters.UserName != "user" || loginParameters.Password != "password")
        {
            return Unauthorized("Invalid username or password");
        }

        // TODO
        var secretKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("ThisIsASecretKeyForTestingItNeedsToBeASecretKeyForTestingIt"));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        // TODO
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, loginParameters.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Token 
        var token = new JwtSecurityToken(
            issuer: "TestIssuer",
            audience: "TestAudience",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30), // Token 30 Minuten gültig
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.WriteToken(token);

        var response = new TokenDto() { Token = jwt };
        return Ok(response);
    }
}