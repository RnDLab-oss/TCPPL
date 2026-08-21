using API;
using API.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Model;
using Model;
using Operation;
using Operation.IOperation;
using Repository;
using Repository.IRepository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// Add services to the container
// ==========================================

builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// ==========================================
// JWT Authentication
// ==========================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ==========================================
            // Validate JWT Issuer
            // ==========================================
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],

            // ==========================================
            // Validate JWT Audience
            // ==========================================
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],

            // ==========================================
            // Validate Token Expiry
            // ==========================================
            ValidateLifetime = true,

            // ==========================================
            // Validate Signing Key
            // ==========================================
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JWT:Secret"]!
                )
            ),

            // Token expire hote hi invalid
            ClockSkew = TimeSpan.Zero
        };
    });


// ==========================================
// Authorization
// ==========================================

builder.Services.AddAuthorization();


// ==========================================
// Swagger
// ==========================================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,

        // IMPORTANT
        Scheme = "bearer",

        BearerFormat = "JWT",
        In = ParameterLocation.Header,

        Description =
            "Enter JWT token only. Do not add 'Bearer'."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddEndpointsApiExplorer();


// ==========================================
// HTTP Context
// ==========================================

builder.Services.AddHttpContextAccessor();


// ==========================================
// Database
// ==========================================

builder.Services.AddDbContext<TcpplWebContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("TcplConString")));


// ==========================================
// Database Helper
// ==========================================

builder.Services.AddSingleton(new DatabaseHelper(builder.Configuration.GetConnectionString("TcplConString")));
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

// ==========================================
// Repository
// ==========================================

builder.Services.AddTransient<
    IStoredProcedureRepository,
    StoredProcedureRepository
>();


// ==========================================
// Operations
// ==========================================

builder.Services.AddTransient<IAuthOperation,AuthOperation>();

// ==========================================
// Exception Filter
// ==========================================

builder.Services.AddScoped<GlobalExceptionFilter>();


// ==========================================
// Build Application
// ==========================================

var app = builder.Build();


// ==========================================
// Swagger
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ==========================================
// HTTPS
// ==========================================

app.UseHttpsRedirection();


// ==========================================
// Authentication
// ==========================================

app.UseAuthentication();


// ==========================================
// Session Validation
// ==========================================

app.UseMiddleware<SessionValidationMiddleware>();


// ==========================================
// API Logging Middleware
// ==========================================

app.UseMiddleware<ApiLoggingMiddleware>();


// ==========================================
// Authorization
// ==========================================

app.UseAuthorization();


// ==========================================
// Controllers
// ==========================================

app.MapControllers();


// ==========================================
// Run Application
// ==========================================

app.Run();