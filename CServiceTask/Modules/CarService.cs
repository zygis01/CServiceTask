using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServiceTask.Modules
{
    public class CarService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Mechanic> Mechanics { get; set; }
        public ICollection<Client> Clients { get; set; }

        public CarService(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is required");
            }
            Name = name;
            Clients = new List<Client>();
            Mechanics = new List<Mechanic>();
        }
        public CarService() { }
    }
}
