using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BankTransactionsManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardAuthTokenHandler>();
builder.Services.AddScoped<VerificationProxyService>();
builder.Services.AddHttpClient("Verification", client =>
{
    client.BaseAddress = new Uri("http://localhost:5140/");
})
.AddHttpMessageHandler<ForwardAuthTokenHandler>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration")))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanWithdraw", policy =>
        policy.RequireClaim("permission", "withdraw"));
    options.AddPolicy("CanCheck", policy =>
        policy.RequireClaim("permission", "check"));
});
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginPolicy", context =>
    {
        // Get remote IP
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        // Get user identifier (if authenticated)
        var user = context.User?.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : "anonymous";
        // Combine IP and user
        var partitionKey = $"{ip}:{user}";

        return RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: partitionKey,
            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                ReplenishmentPeriod = TimeSpan.FromMinutes(15),
                TokensPerPeriod = 3,
                AutoReplenishment = true
            });
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankTransactionsManagement API", Version = "v1" });
    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankTransactionsManagement API V1");
        c.RoutePrefix = "swagger"; // Swagger UI at root
    });
}
app.UseMiddleware<BankTransactionsManagement.Middleware.ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();