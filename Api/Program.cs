using Api;
using Api.Entities;
using Api.Events;
using Api.Extensions;
using Api.Projections;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Marten;
using Marten.Events.Projections;
using Oakton;
using Weasel.Core;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

var signingKey = builder.Configuration.GetSection("jwt:signingKey").Value;

// controller api with fastEndpoints easier for me :)
builder.Services
    .Configure<JwtCreationOptions>(o => o.SigningKey = signingKey!)
   .AddAuthenticationJwtBearer(s => s.SigningKey = signingKey!)
   .AddAuthorization()
   .AddFastEndpoints(
    )
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

// For now, this is enough to integrate Wolverine into
// your application, but there'll be *many* more
// options later of course :-)
builder.Host.UseWolverine(opts =>
{
    opts.UseFluentValidation();

    opts.Services.AddMarten(options =>
    {
        // Establish the connection string to your Marten database
        options.Connection(builder.Configuration.GetConnectionString("Marten")!);

        options.Events.AddEventType<AccountCreated>();
        options.Events.AddEventType<MoneyDeposited>();
        options.Events.AddEventType<MoneyWithdrawn>();
        options.Events.AddEventType<MoneyTransfered>();
        options.Events.AddEventType<AccountClosed>();

        // Specify that we want to use STJ as our serializer
        options.UseSystemTextJsonForSerialization();

        // If we're running in development mode, let Marten just take care
        // of all necessary schema building and patching behind the scenes
        if (builder.Environment.IsDevelopment())
        {
            options.AutoCreateSchemaObjects = AutoCreate.All;
        }

        options.Projections.Add<BankAccountProjection>(ProjectionLifecycle.Inline);
        //options.Projections.Add<BankAccountTransactionsProjection>(ProjectionLifecycle.Inline);
        // Register the Movie document
        options.Schema.For<BankAccount>().Identity(x => x.Id);
    })
    .UseLightweightSessions()
    .IntegrateWithWolverine(); // Ensures session handling is correct\


    opts.Policies.AutoApplyTransactions();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.SeedData();
}


app.UseAuthentication()
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
/*
app.MapGet("/accounts", async (IMessageBus bus, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
{
    page = int.Max(1, page);
    pageSize = int.Max(1, pageSize);

    return await bus.InvokeAsync<IEnumerable<BankAccount>>(new ListBankAccountsQuery(page, pageSize));
});

app.MapGet("/accounts/{id}/transactions", async (IMessageBus bus, Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
{
    page = int.Max(1, page);
    pageSize = int.Max(1, pageSize);

    return await bus.InvokeAsync<IEnumerable<BankAccountTransaction>>(new ListBankAccountTransactionsQuery(id, page, pageSize));
})
    .WithTags("Accounts");

app.MapGet("/transactions", async (IMessageBus bus, [FromQuery] int page = 1, [FromQuery] int pageSize = 100) =>
{
    page = int.Max(1, page);
    pageSize = int.Max(1, pageSize);

    return await bus.InvokeAsync<IEnumerable<BankAccountTransaction>>(new AllTransactionsQuery(page, pageSize));
});
*/

app.MapGet("/", () => Results.Redirect("/swagger"));
/*
app.MapWolverineEndpoints(opts =>
{
    opts.UseFluentValidationProblemDetailMiddleware();
});*/

// Opt into using Oakton for command line parsing
// to unlock built in diagnostics and utility tools within
// your Wolverine application
return await app.RunOaktonCommands(args);