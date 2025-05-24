using Core.Interfaces;
using Core.Middlewares;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
}); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger config
//builder.Services.AddSwaggerGen();
string instance = builder.Configuration["AzureAdB2C:Instance"]!;
string tenantId = builder.Configuration["AzureAdB2C:TenantId"]!;
string clientid = builder.Configuration["AzureAdB2C:ClientId"]!;
//string ApplicationIdURI = $"api://{builder.Configuration["AzureAdB2C:ApplicationIdURI"]!}";
string ApplicationIdURI = $"api://{tenantId}/{builder.Configuration["AzureAdB2C:ApplicationIdURI"]!}";
string scope = "default";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Configure OAuth2
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{instance}{tenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"{instance}{tenantId}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { $"api://ThresholdAlertBackEnd/{scope}", "user Access API" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { $"api://ThresholdAlertBackEnd/{scope}" }
        }
    });
});
#endregion



// Adds Microsoft Identity platform (Azure AD B2C) support to protect this Api
/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(options =>
        {
            builder.Configuration.Bind("AzureAdB2C", options);

            options.TokenValidationParameters.NameClaimType = "name";
        },
options => { builder.Configuration.Bind("AzureAdB2C", options); });
*/
// End of the Microsoft Identity platform block 

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminAccess", policy =>
        policy.RequireClaim("jobTitle", "admin"));
});


builder.Services.Configure<EmailNotificationServiceOptions>(
    builder.Configuration.GetSection(EmailNotificationServiceOptions.EmailNotificationService));

builder.Services.AddSingleton<INotificationService,EmailNotificationService>();

builder.Services.AddScoped<IMeasurementRepository,MeasurementRepository>();
builder.Services.AddScoped<IMeasurementService,MeasurementService>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddSingleton<AlertService>();
builder.Services.AddSingleton<TimedHostedService>();
builder.Services.AddSingleton<KeepAliveHostedService>();

builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

// Configure the SQLite connection
string connectionString = builder.Configuration["ConnectionStrings:SQLiteConnectionString"]!;
Console.WriteLine(connectionString);
var connection = new SqliteConnection(connectionString);
connection.Open();

// Set journal mode to DELETE using PRAGMA statement
using (var command = connection.CreateCommand())
{
    command.CommandText = "PRAGMA journal_mode = DELETE;";
    command.ExecuteNonQuery();
}

builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptions => dbContextOptions.UseSqlite(connection));

// Add CORS services
string[] allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? "").Split(",");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(allowedOrigins) // React dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

#region Apply EF migrations
using (var serviceScopescope = app.Services.CreateScope())
{
    var dbContext = serviceScopescope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
#endregion



app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
#region Swagger pipeline config
string spaClientId = builder.Configuration["AzureAdB2C:SpaClientId"]!;
app.UseSwagger();
var oAuthRedirectUrl = builder.Configuration["AzureAdB2C:RedirectUri"];
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.OAuthClientId($"{spaClientId}");
    c.OAuthUsePkce();  // Recommended for B2C
    c.OAuth2RedirectUrl(oAuthRedirectUrl); // Same as the one registered in Azure B2C
    c.OAuthScopes($"api://ThresholdAlertBackEnd/{scope}"); //Selects this scope by default.
});
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();
// Use CORS
    app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
