using CServiceTask.Modules;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServiceTask.Migrations
{
    public class carServiceContext : DbContext
    {
        public DbSet<CarService> CarServices { get; set; }
        public DbSet<Mechanic> Mechanics { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            options.UseSqlServer($@"Data Source=DESKTOP-67758C1\SQLEXPRESS;Initial Catalog=DbTaskDatabase;Integrated Security=True");
        }
    }
}
