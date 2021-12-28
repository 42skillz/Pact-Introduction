namespace ConsumerEmployee
{
    public class Employee
    {
        public Employee()
        {
        }

        public Employee(int id, string name, string city)
        {
            Id = id;
            Name = name;
            City = city;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }
    }
}
