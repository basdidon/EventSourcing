using Api.Commands;
using Api.DTOs;
using Api.Entities;
using Api.Events;
using Api.Projections;
using Api.Queries;
using JasperFx.Core;
using Marten;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Mvc;
using Oakton;
using Weasel.Core;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;
using Wolverine.Http.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// The almost inevitable inclusion of Swashbuckle:)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

        // Register the Movie document
        options.Schema.For<BankAccount>().Identity(x => x.Id);
    })
    .UseLightweightSessions()
    .IntegrateWithWolverine(); // Ensures session handling is correct\
    

    opts.Policies.AutoApplyTransactions();
});

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.MapGet("/accounts", async (IMessageBus bus,[FromQuery]int page = 1, [FromQuery]int pageSize = 10) =>
{
    page = int.Max(1, page);
    pageSize = int.Max(1, pageSize);

    return await bus.InvokeAsync<IEnumerable<BankAccount>>(new ListBankAccountsQuery(page,pageSize));
});

app.MapGet("/accounts/{id}/transactions", async (IMessageBus bus,Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
{
    page = int.Max(1, page);
    pageSize = int.Max(1, pageSize);

    return await bus.InvokeAsync<IEnumerable<BankAccountTransaction>>(new ListBankAccountTransactionsQuery(id, page, pageSize));


});

app.MapPost("/accounts", (CreateAccountCommand body, IMessageBus bus)
    => bus.InvokeAsync<BankAccount>(body));

app.MapPost("/accounts/{id}/deposit", (Guid id, DepositCommand body, IMessageBus bus) 
    => bus.InvokeAsync(body with { AccountId = id}));

app.MapPost("/accounts/{id}/withdraw", (Guid id, WithdrawCommand body, IMessageBus bus)
    => bus.InvokeAsync(body with { AccountId = id}));

// Swashbuckle inclusion
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapWolverineEndpoints(opts =>
{
    opts.UseFluentValidationProblemDetailMiddleware();
});
// Opt into using Oakton for command line parsing
// to unlock built in diagnostics and utility tools within
// your Wolverine application
return await app.RunOaktonCommands(args);