using CServiceTask.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServiceTask.Interfaces
{
    public interface IFunctions
    {
        public void CreateCarService();
        public void AddMechanicToService();
        public void CreateClientToService();
        public void AddMechanicAndClientToService();
        public void UpdateMechanicAndClients();
        public void UpdateMechanicService();


    }
}
