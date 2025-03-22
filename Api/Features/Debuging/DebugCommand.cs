using FastEndpoints;

namespace Api.Features.Debuging
{
    public class DebugCommand : ICommand { }
    /*
    public class DebugCommandHandler(ILogger<DebugCommandHandler> logger) : ICommandHandler<DebugCommand>
    {
        public async Task ExecuteAsync(DebugCommand command, CancellationToken ct)
        {
            logger.LogInformation("[FastEndpoints] DebugCommand executed.");
            await Task.CompletedTask;
        }
        //
        public void Handle(DebugCommand command)
        {
            Console.WriteLine("[WolverineFx] DebugCommand Handled.");
        }
    }
    /*
    public class DebugCommandWolverineHandler
    {
        public void Handle(DebugCommand command)
        {
            Console.WriteLine("[WolverineFx] DebugCommand Handled.");
        }
    }*/
}
