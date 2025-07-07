using Greenlogist.Application.Interfaces;
using Greenlogist.Infrastructure.Security;
using Greenlogist.Infrastructure.Persistence.Repositories;
using Greenlogist.Domain.Repositories;
using MediatR;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Greenlogist.Application.Commands.Shipping;
using Greenlogist.Application.Commands.Order;
using Greenlogist.Application.Queries.Statistics; 

var builder = WebApplication.CreateBuilder(args);

// JWT configuration from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey not configured."));

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Only for development, should be true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // No time leeway for expiration
    };
});

// Add services to the dependency injection container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Greenlogist API", Version = "v1" });

    // JWT Configuration in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text field below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1Ni...\"",
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
            Array.Empty<string>()
        }
    });

    // Configure Swagger to use XML comments from controllers and DTOs
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Also include XML comments from other projects if they have them (e.g., Application layer DTOs)
    var applicationXmlFile = typeof(Greenlogist.Application.Common.ApplicationException).Assembly.GetName().Name + ".xml";
    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
    if (File.Exists(applicationXmlPath))
    {
        c.IncludeXmlComments(applicationXmlPath);
    }
});

// Enable XML comment generation for Swagger
builder.Services.AddOptions<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>()
    .Configure(options =>
    {
        options.SuppressMapClientErrors = true;
    });

// Dependency injection for the Infrastructure layer
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>(); // Singleton because it's stateless
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>(); // Singleton

// Dependency injection for the Domain layer (Repositories)
builder.Services.AddSingleton<IUserRepository, UserRepository>(); // Singleton for in-memory simulation
builder.Services.AddSingleton<IProductRepository, ProductRepository>(); // Register Product Repository
builder.Services.AddSingleton<IShippingRequestRepository, ShippingRequestRepository>(); // Register Shipping Request Repository
builder.Services.AddSingleton<IOrderRepository, OrderRepository>(); // Register Order Repository

// Dependency injection for MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())); // Registers handlers from the API assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Greenlogist.Application.Commands.Auth.RegisterUserCommand).Assembly)); // Registers handlers from the Application layer assembly (Auth)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Greenlogist.Application.Commands.Product.RegisterProductCommand).Assembly)); // Registers handlers from the Application layer assembly (Product)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Greenlogist.Application.Commands.Shipping.SolicitTransportCommand).Assembly)); // Registers handlers from the Application layer assembly (Shipping)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Greenlogist.Application.Commands.Order.PlaceOrderCommand).Assembly)); // Registers handlers from the Application layer assembly (Order)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Greenlogist.Application.Queries.Statistics.GetProducerDashboardSummaryQuery).Assembly)); // NEW: Registers handlers from the Application layer assembly (Statistics)


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Greenlogist API V1");
        c.RoutePrefix = string.Empty; // Make Swagger UI the home page
    });
}

app.UseHttpsRedirection();

// Enable CORS (adjust this according to your production needs)
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication(); // Must go before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
