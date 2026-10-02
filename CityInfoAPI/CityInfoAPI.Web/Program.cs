using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CityInfoAPI.Controllers.Filters;
using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.PropertyMapping;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Service;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

#pragma warning disable CS1591

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog();

//--LOGGING--//
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.MSSqlServer
                (
                    connectionString: builder.Configuration["DbConnectionString"],
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        SchemaName = "dbo",
                        AutoCreateSqlTable = true
                    },
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    formatProvider: null,
                    columnOptions: null,
                    logEventFormatter: null
                );

// console and file sinks in development only
if (environment == Environments.Development)
{
    loggerConfiguration
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day);
}

Log.Logger = loggerConfiguration.CreateLogger();



//--SERVICES--//
// Media Types
builder.Services.AddControllers(options =>
{
    //  media types: don't blindly return json regardless of what they asked for.
    options.ReturnHttpNotAcceptable = true;

    options.CacheProfiles.Add("5MinuteCacheProfile",
                                new CacheProfile()
                                {
                                    Duration = 300, // 5 minutes
                                    Location = ResponseCacheLocation.Any,

                                    // cache by query string
                                    // what does this do?
                                    VaryByQueryKeys = new[] { "*" }
                                });
})

// replaces default json input and output formatters with Json.NET
.AddNewtonsoftJson(options =>
{
    //configure your JSON serializer to handle or ignore self-referencing loops.
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
})

// enables XML input and output formatters
.AddXmlDataContractSerializerFormatters();

// adding to the default ProblemDetailsResponse
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Extensions["MachineName"] = Environment.MachineName;
    };
});

// sets content type to return based on file extension of file.
// custom services: inject interfaceX, provide an implementation of concrete type Y
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<IMailService, CloudMailService>();
builder.Services.AddDbContext<CityInfoDbContext>(dbContextOptions => dbContextOptions.UseSqlServer(builder.Configuration["DbConnectionString"]));
builder.Services.AddHealthChecks().AddDbContextCheck<CityInfoDbContext>();
builder.Services.AddScoped<IStatesRepository, StatesRepository>();
builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();
builder.Services.AddScoped<IPointsOfInterestRepository, PointsOfInterestRepository>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IPointsOfInterestService, PointsOfInterestService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IPropertyMappingProcessor, PropertyMappingProcessor>();
builder.Services.AddScoped<CheckForExistingCityNameFilter>();


// add caching / cache store
builder.Services.AddResponseCaching();

// AutoMapper.  Scan for profiles.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddAuthentication(); ??

// token - configure how we will validate the token
builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],

                // this is the same logic as we used creating the signature in the auth controller,
                // therefore we know it matches.
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(
                    builder.Configuration["Authentication:SecretForKey"]
                    ?? throw new InvalidOperationException("Configuration value 'Authentication:SecretForKey' is missing.")))
            };
        });

//// DEMO ONLY: adding a authorization policy
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("MustBeFromRichmond", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        policy.RequireClaim("city", "Richmond");
//    });
//});
//// end DEMO

// versioning
builder.Services.AddApiVersioning(setUpAction =>
{
    setUpAction.ReportApiVersions = true;
    setUpAction.AssumeDefaultVersionWhenUnspecified = true;
    setUpAction.DefaultApiVersion = new ApiVersion(1, 0);
})
.AddMvc()
.AddApiExplorer(setUpAction =>
{
    setUpAction.GroupNameFormat = "'v'VVV";
    setUpAction.SubstituteApiVersionInUrl = true;
});

// built-in OpenAPI (core AddOpenApi, so the XML comment generator can intercept it): one document per API version, viewed through Scalar
builder.Services.AddOpenApi("v1", options =>
{
    // adding the security definition for the UI to use, and requiring it on every operation
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "CityInfo API";
        document.Info.Version = "1.0";
        document.Info.Description = "Through this API you can access cities and points of interest.";
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["CityInfoAPIBearerAuth"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Description = "Input a valid token to access this API."
        };
        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("CityInfoAPIBearerAuth", document)] = []
            }
        ];
        return Task.CompletedTask;
    });
});

// add header forwarding
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


//--APPLICATION--//
var app = builder.Build();

app.UseResponseCaching();

// Configure the HTTP request pipeline. //
// for now, since this is a demo, let's expose the errors and the API reference in every environment.
app.UseDeveloperExceptionPage();

// give bodiless 401/403/404/406 etc. a ProblemDetails body (uses the AddProblemDetails registration above)
app.UseStatusCodePages();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    var descriptions = app.DescribeApiVersions();
    for (var i = 0; i < descriptions.Count; i++)
    {
        var description = descriptions[i];
        options.AddDocument(description.GroupName, description.GroupName.ToUpperInvariant(), isDefault: i == descriptions.Count - 1);
    }
});

app.UseForwardedHeaders();

app.UseHttpsRedirection();

// add routing middleware to request pipeline
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// MapControllers will add endpoints to controller actions by using attributes
app.MapControllers();

app.MapHealthChecks("/api/health");

app.Run();

#pragma warning restore CS1591
