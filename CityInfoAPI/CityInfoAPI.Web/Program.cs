using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Service;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

#pragma warning disable CS1591

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog();

//--LOGGING--//
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environment == Environments.Development)
{
    Log.Logger = new LoggerConfiguration()
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
                    )
                    .WriteTo.Console()
                    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
}
else
{
    Log.Logger = new LoggerConfiguration()
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
                )
                .CreateLogger();
}



//--SERVICES--//
// Media Types
builder.Services.AddControllers(options =>
{
    //  media types: don't blindly return json regardless of what they asked for.
    options.ReturnHttpNotAcceptable = true;
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
        context.ProblemDetails.Extensions.Add("MachineName", Environment.MachineName);
    };
});

// sets content type to return based on file extension of file.
// custom services: inject interfaceX, provide an implementation of concrete type Y
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<IMailService, CloudMailService>();
builder.Services.AddDbContext<CityInfoDbContext>(dbContextOptions => dbContextOptions.UseSqlServer(builder.Configuration["DbConnectionString"]));
builder.Services.AddHealthChecks().AddDbContextCheck<CityInfoDbContext>();
builder.Services.AddScoped<IStatesRepository, StatesRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IPointsOfInterestRepository, PointsOfInterestRepository>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IPointsOfInterestService, PointsOfInterestService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



// AutoMapper.  Scan for profiles.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// swagger, swashbuckle
builder.Services.AddEndpointsApiExplorer();

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
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]))
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


var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
builder.Services.AddSwaggerGen(setUpAction =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        setUpAction.SwaggerDoc($"{description.GroupName}",
                                new()
                                {
                                    Title = "CityInfo API",
                                    Version = description.ApiVersion.ToString(),
                                    Description = "Through this API you can access cities and points of interest."
                                });
    }

    // since multiple projects will have xml documentation, we will need to loop thru all the files and include
    // all of the xml docs....not just the CityInfoAPI.Web.Xml.
    // **for some reason, these files are not picked up on Azure.**
    DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
    foreach (var fileInfo in baseDirectoryInfo.EnumerateFiles("CityInfoAPI*.xml"))
    {
        setUpAction.IncludeXmlComments(fileInfo.FullName);
    };

    // adding the security definition for the swagger UI to use
    setUpAction.AddSecurityDefinition("CityInfoAPIBearerAuth", new()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Input a valid token to access this API."
    });

    // automatically send the bearer token in the authorization header in the swagger UI.
    setUpAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CityInfoAPIBearerAuth"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddHealthChecks();

// add header forwarding
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});



//--APPLICATION--//
var app = builder.Build();

// Configure the HTTP request pipeline. //
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler(appBuilder =>
    //{
    //    appBuilder.Run(async context =>
    //    {
    //        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    //        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
    //    });
    //});

    // for now, since this is a demo, let's expose the errors and swagger in production.
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(setUpAction =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            setUpAction.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(setUpAction =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            setUpAction.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseForwardedHeaders();

app.UseHttpsRedirection();

// add routing middleware to request pipeline
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// MapControllers will add endpoints to controller actions by using attributes
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.MapHealthChecks("/api/health");

app.Run();

#pragma warning restore CS1591
