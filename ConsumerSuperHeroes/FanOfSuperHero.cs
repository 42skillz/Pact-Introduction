namespace ConsumerSuperHeroes
{
    public class FanOfSuperHero
    {
        public FanOfSuperHero(int id, string firstName, string name, string summary)
        {
            Id = id;
            FirstName = firstName;
            Name = name;
            Summary = summary;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
    }
}