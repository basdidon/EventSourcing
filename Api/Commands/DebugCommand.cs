using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace Api.Commands
{
    public class DebugCommand : ICommand { }

    public class DebugCommandHandler(ILogger<DebugCommandHandler> logger) : ICommandHandler<DebugCommand>
    {
        public async Task ExecuteAsync(DebugCommand command, CancellationToken ct)
        {
            logger.LogInformation("[FastEndpoints] DebugCommand executed.");
            await Task.CompletedTask;
        }
    }
}
