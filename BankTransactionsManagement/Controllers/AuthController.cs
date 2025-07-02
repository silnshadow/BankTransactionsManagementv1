using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankTransactionsManagement.Controllers
{
    
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private VerificationProxyService _verificationProxyService{ get; }
        public AuthController(IConfiguration configuration, VerificationProxyService verificationProxyService)
        {
            _configuration = configuration;
            _verificationProxyService = verificationProxyService ?? throw new ArgumentNullException(nameof(verificationProxyService));
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Dummy authentication logic (replace with real user validation)
            if (model.Username == "admin" && model.Password == "password")
            {
                var token = GenerateJwtToken(model.Username);
                var kafkaProducer = new KafkaProducerService();
                // await kafkaProducer.SendUserLoginEventAsync(new UserLoginEvent
                // {
                //     UserId = model.Username,
                //     Email = "email@gmail.com",
                //     LoginTime = DateTime.UtcNow
                // });
                return Ok(new { token });
            }
            return Unauthorized();
        }
        [Authorize]
        [HttpPost("initiateverification")]
        public async Task<IActionResult> InitiateVerification([FromBody] LoginModel model)
        {
            // Dummy authentication logic (replace with real user validation)
            if (model.Username == "admin")
            {
                var result = await _verificationProxyService.GetVerificationInitiationAsync(model.Username);
                return Ok(result);
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("permission", "withdraw"),
                new Claim("permission", "check"),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}