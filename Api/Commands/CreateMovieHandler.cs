using EventSourcing.Entities;
using Marten;
using System.Threading.Tasks;

namespace EventSourcing.Commands
{
    public static class CreateMovieHandler
    { 
        public static void Handle(CreateMovie command, IDocumentSession session)
        {
            var movie = new Movie
            {
                Id = command.Id,
                Name = command.Name,
                ReleaseDate = command.ReleaseDate,
            };

            Console.WriteLine("################## Create Movie ########################");
            Console.WriteLine($"Name:{command.Name}");
            Console.WriteLine("########################################################");

            session.Store(movie);
            //await session.SaveChangesAsync();
        }
    }
}