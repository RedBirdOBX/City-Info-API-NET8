using Microsoft.AspNetCore.StaticFiles;
using Serilog.Events;
using Serilog;
using CityInfoAPI.Web.Services;
using CityInfoAPI.Data;
using CityInfoAPI.Data.DbContents;
using Microsoft.EntityFrameworkCore;
using CityInfoAPI.Data.Repositories;
using Microsoft.IdentityModel.Tokens;
using Asp.Versioning;


//--LOGGING--//
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                //.WriteTo.MSSqlServer
                //(
                //    connectionString: builder.Configuration["ConnectionStrings:drummersDbConnectionString"],
                //    sinkOptions: new MSSqlServerSinkOptions
                //    {
                //        TableName = "Logs",
                //        SchemaName = "dbo",
                //        AutoCreateSqlTable = true
                //    },
                //    restrictedToMinimumLevel: LogEventLevel.Information,
                //    formatProvider: null,
                //    columnOptions: null,
                //    logEventFormatter: null
                //)
                .WriteTo.Console()
                .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();


var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog();


//--SERVICES--//
// Media Types
builder.Services.AddControllers(options =>
{
    //  media types: don't blindly return json regardless of what they asked for.
    options.ReturnHttpNotAcceptable = true;
})

// replaces default json input and output formatters with Json.NET
.AddNewtonsoftJson()

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
builder.Services.AddSingleton<CityInfoMemoryDataStore>();
builder.Services.AddDbContext<CityInfoDbContext>(dbContextOptions => dbContextOptions.UseSqlServer(builder.Configuration["DbConnectionString"]));
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

// AutoMapper.  Scan for profiles.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// swagger, swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// token
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
}).AddMvc();


// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0
builder.Services.AddHealthChecks();


//--APPLICATION--//
var app = builder.Build();

// Configure the HTTP request pipeline. //
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// add routing middleware to request pipeline
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// MapControllers will add endpoints to controller actions by using attributes
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.MapHealthChecks("/api/health");

app.Run();
