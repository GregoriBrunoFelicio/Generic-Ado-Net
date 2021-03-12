using System.Threading.Tasks;

namespace Generic.Ado.Net
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var person = new Person
            {
                Name = "ASDASDASDSAD",
                Age = 1023
            };

            var repo = new Repository<Person>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Person;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            var affectedRows = await repo.InsertAsync(person);
            var persons = repo.GetAllAsync();
        }
    }

    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }


}
