using CServiceTask.Modules;
using System;

namespace CServiceTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StartupModule start = new StartupModule();
            start.Return();
        }
    }
}
