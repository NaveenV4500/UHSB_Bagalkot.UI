using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;


var builder = WebApplication.CreateBuilder(args);
// Add EF Core with SQL Server
builder.Services.AddDbContext<UHSB_Bagalkot.Data.Models.Uhsb2025uatContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Register your repository for DI
builder.Services.AddScoped<farmerRepository, farmerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IHorticultureHandbookRepository, HorticultureHandbookRepository>();
builder.Services.AddScoped<ICropProfileRepository, CropProfileRepository>();
builder.Services.AddScoped<IWeatherCastRepository, WeatherCastRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<ICetegoryRepository, CetegoryRepository>();
builder.Services.AddScoped<IAvailabilityToolsRepository, AvailabilityToolsRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IEmailService, EmailService>(); 
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperConfig>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();


// Add CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigins",
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:3000", "http://localhost:8081/", "http://localhost/UHSB", "http://localhost:8081/") // allowed origins
//                  .AllowAnyHeader()
//                  .AllowAnyMethod(); // or .WithMethods("GET","POST") to restrict
//        });
//});
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllOrigins",
//        builder =>
//        {
//            builder
//                .AllowAnyOrigin()    // For dev/test; use specific origins in production
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//        });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Bind JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InwardsInvoices", "TempFiles")),
//    RequestPath = "/InwardsInvoices/TempFiles"
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Use CORS
app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(@"D:\InwardsInvoices\TempFiles"),
//    RequestPath = "/InwardsInvoices/TempFiles"
//});

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(@"D:\WeatherReportFiles\TempFiles"),
//    RequestPath = "/WeatherReportFiles/TempFiles"
//});
var inwardPath = @"D:\InwardsInvoices\TempFiles";
var weatherPath = @"D:\WeatherReportFiles\TempFiles";

if (!Directory.Exists(inwardPath))
    Directory.CreateDirectory(inwardPath);

if (!Directory.Exists(weatherPath))
    Directory.CreateDirectory(weatherPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(inwardPath),
    RequestPath = "/InwardsInvoices/TempFiles"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(weatherPath),
    RequestPath = "/WeatherReportFiles/TempFiles"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(weatherPath),
    RequestPath = "/InwardsInvoices/TempFiles/Crops"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(weatherPath),
    RequestPath = "/InwardsInvoices/TempFiles/CropsItems"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(weatherPath),
    RequestPath = "/InwardsInvoices/TempFiles/Sections"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(weatherPath),
    RequestPath = "/InwardsInvoices/TempFiles/Category"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(weatherPath),
    RequestPath = "/InwardsInvoices/TempFiles/Content"
});
app.Run();
