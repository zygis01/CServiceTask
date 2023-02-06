using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServiceTask.Modules
{
    public class Mechanic
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public int CarServiceId { get; set; }
        [ForeignKey("CarServiceId")]
        public virtual CarService CarService { get; set; }
        public ICollection<Client> Clients { get; set; }

        public Mechanic(string firstName, string title)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("First name is required");
            }
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Title is required");
            }
            FirstName = firstName;
            Title = title;
            Clients = new List<Client>();
        }
    }
}
