/*
using FastEndpoints;
using Wolverine;

namespace Api.Features.Debuging
{
    public class DebugCommandEndpoint(IMessageBus bus) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Post("debug");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await new DebugCommand().ExecuteAsync(ct);
            await bus.InvokeAsync(new DebugCommand());
        }
    }
}
*/