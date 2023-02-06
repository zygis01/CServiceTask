using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServiceTask.Modules
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<CarService> Services { get; set; }
        public ICollection<Mechanic> Mechanics { get; set; }

        public Client(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("First name is required");
            }
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("First name is required");
            }
            FirstName = firstName;
            LastName = lastName;
            Services = new List<CarService>();
            Mechanics = new List<Mechanic>();
        }
        public Client() { }
    }
}
