namespace EventSourcing.Entities
{
    public class Movie
    {
        public Guid  Id { get; set; }

        public string Name { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
    }
}
