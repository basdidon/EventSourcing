using EventSourcing.Commands;
using EventSourcing.Entities;
using Marten;
using Oakton;
using Weasel.Core;
using Wolverine;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

// The almost inevitable inclusion of Swashbuckle:)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// For now, this is enough to integrate Wolverine into
// your application, but there'll be *many* more
// options later of course :-)
builder.Host.UseWolverine(opts =>
{
    opts.Services.AddMarten(options =>
    {
        // Establish the connection string to your Marten database
        options.Connection(builder.Configuration.GetConnectionString("Marten")!);

        // Specify that we want to use STJ as our serializer
        options.UseSystemTextJsonForSerialization();

        // If we're running in development mode, let Marten just take care
        // of all necessary schema building and patching behind the scenes
        if (builder.Environment.IsDevelopment())
        {
            options.AutoCreateSchemaObjects = AutoCreate.All;
        }

        // Register the Movie document
        options.Schema.For<Movie>().Identity(x => x.Id);
    })
.IntegrateWithWolverine(); // Ensures session handling is correct
    opts.Policies.AutoApplyTransactions();
});

var app = builder.Build();

// An endpoint to create a new issue that delegates to Wolverine as a mediator
//app.MapPost("/issues/create", (CreateIssue body, IMessageBus bus) => bus.InvokeAsync(body));

// An endpoint to assign an issue to an existing user that delegates to Wolverine as a mediator
//app.MapPost("/issues/assign", (AssignIssue body, IMessageBus bus) => bus.InvokeAsync(body));

app.MapGet("/movies/{id}",  async (Guid id, IMessageBus bus) =>
{
    var movie = await bus.InvokeAsync<Movie>(new GetMovieByIdQuery(id));

    return movie;
});
app.MapPost("/movies", (CreateMovie body, IMessageBus bus) => bus.InvokeAsync(body));

// Swashbuckle inclusion
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

// Opt into using Oakton for command line parsing
// to unlock built in diagnostics and utility tools within
// your Wolverine application
return await app.RunOaktonCommands(args);