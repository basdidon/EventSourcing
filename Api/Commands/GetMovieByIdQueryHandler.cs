using EventSourcing.Entities;
using Marten;

namespace EventSourcing.Commands
{
    public static class GetMovieByIdQueryHandler
    { 
        public static async Task<Movie?> Handle(GetMovieByIdQuery query,IQuerySession session,CancellationToken ct)
        {
            return await session.Query<Movie>()
                .FirstOrDefaultAsync(m => m.Id == query.Id, ct);
        }
    }
}