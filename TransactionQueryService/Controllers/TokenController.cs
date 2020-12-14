using Glasswall.Administration.K8.TransactionQueryService.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Glasswall.Administration.K8.TransactionQueryService.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Glasswall.Administration.K8.TransactionQueryService.Controllers
{
    [ApiController]
    [Route("api/v1/token")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITransactionQueryServiceConfiguration _configuration;

        public TokenController(ILogger<TokenController> logger, ITransactionQueryServiceConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        [BasicAuth]
        public IActionResult GetToken()
        {
            _logger.LogInformation("Authenticating user {0}", _configuration.Username);
            return Ok(GenerateToken(_configuration.Username));
        }
        
        public string GenerateToken(string username)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.TokenSecret));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = "auth-app",
                Audience = "any",
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}