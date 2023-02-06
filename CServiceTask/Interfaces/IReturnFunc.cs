using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServiceTask.Interfaces
{
    public interface IReturnFunc
    {
        public void ReturnAllServiceMechanics();
        public void ReturnAllServiceClients();
        public void ReturnAllClientsByMechanic();
    }
}
