using Api;
using Api.Events;
using Api.Events.User;
using Api.Extensions;
using Api.Features.Accounts;
using Api.Features.Accounts.Withdraw;
using Api.Features.Users;
using Api.Features.Users.Auth.RefreshToken;
using Api.Persistance;
using Api.Services;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using JasperFx.Core;
using Marten;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

var identityDb = builder.Configuration.GetConnectionString("identityDb");
var signingKey = builder.Configuration.GetSection("jwt:signingKey").Value;

builder.Services.AddTransient<RoleService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<OtpService>();
builder.Services.AddTransient<SmsService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(identityDb);
});

builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    // Turn on the PostgreSQL table partitioning for
    // hot/cold storage on archived events
    options.Events.UseArchivedStreamPartitioning = true;

    options.Events.AddEventType<UserRegistered>();
    options.Events.AddEventType<AccountCreated>();
    options.Events.AddEventType<MoneyDeposited>();
    options.Events.AddEventType<MoneyWithdrawn>();
    options.Events.AddEventType<MoneySent>();
    options.Events.AddEventType<MoneyReceived>();
    options.Events.AddEventType<AccountClosed>();
    options.Events.AddEventType<WithdrawRequested>();
    options.Events.AddEventType<WithdrawConfirmed>();
    options.Events.AddEventType<WithdrawRejected>();
    options.Events.AddEventType<WithdrawRevocked>();

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }

    options.Projections.Add<BankAccountProjection>(ProjectionLifecycle.Inline);
    options.Projections.Add<WithdrawalProjection>(ProjectionLifecycle.Inline);
    //options.Projections.Add<BankAccountTransactionsProjection>(ProjectionLifecycle.Inline);
    // Register the Movie document
    options.Schema.For<BankAccount>().Identity(x => x.Id);
    options.Schema.For<Withdrawal>().Identity(x => x.RequestId);
})
    .UseLightweightSessions();

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


// controller api with fastEndpoints easier for me :)
builder.Services
    .Configure<JwtCreationOptions>(o => o.SigningKey = signingKey!)
   .AddAuthenticationJwtBearer(s => s.SigningKey = signingKey!)
   .AddAuthorization()
   .AddFastEndpoints(o=>o.IncludeAbstractValidators = true)
   .SwaggerDocument(o =>
   {
       o.MaxEndpointVersion = 1;
       o.DocumentSettings = s =>
       {
           s.DocumentName = "Initial Release";
           s.Title = "Banking API";
           s.Version = "v1";
       };
   });

// The Default Authentication Scheme
// see more : https://fast-endpoints.com/docs/security#the-default-authentication-scheme
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.EnsureDatabaseCreated<ApplicationDbContext>(resetOnStart: true);
    await app.SeedData();
}

app.UseJwtRevocation<BlacklistChecker>()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
{
    c.Endpoints.Configurator = ep =>
    {
        ep.Options(b => b.AddEndpointFilter<EndpointRequestFilter>());
    };
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
    c.Versioning.PrependToRoute = true;
    c.Versioning.DefaultVersion = 1;
}).UseSwaggerGen();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapHealthChecks("health");

app.Run();

public partial class Program() { }