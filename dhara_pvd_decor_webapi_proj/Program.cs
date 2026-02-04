using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using dhara_pvd_decor_webapi_proj.Services;
using dhara_pvd_decor_webapi_proj.Services.Implementations;
using dhara_pvd_decor_webapi_proj.Services.Interfaces;
using static EmailService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(Options => {
    Options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

//builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "DharaAPI:";
});

builder.Services.AddControllers();


// ================= DEPENDENCY INJECTION =================
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IFinYearService, FinYearService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IColourService, ColourService>();
builder.Services.AddScoped<IMonthService, MonthService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IPayTypeService, PayTypeService>();
builder.Services.AddScoped<ITranTypeService, TranTypeService>();
builder.Services.AddScoped<IProdTypeService, ProdTypeService>();
builder.Services.AddScoped<IProductServices, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IUserServices, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dhara API",
        Version = "v1"
    });

    // JWT Bearer configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT like: Bearer {your_token}"
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
});

// ================= JWT CONFIGURATION =================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var cache = context.HttpContext.RequestServices
                .GetRequiredService<IDistributedCache>();

            var jti = context.Principal?
                .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)
                ?.Value;

            if (string.IsNullOrEmpty(jti))
            {
                context.Fail("Token has no JTI");
                return;
            }

            var redisToken = await cache.GetStringAsync($"jwt:{jti}");

            if (string.IsNullOrEmpty(redisToken))
            {
                // Token not found in Redis
                context.Fail("Token revoked or expired");
            }
        }
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
};

// REQUIRED when behind NGINX / Load Balancer
forwardedHeaderOptions.KnownNetworks.Clear();
forwardedHeaderOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeaderOptions);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
