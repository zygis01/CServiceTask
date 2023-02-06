using CServiceTask.Interfaces;
using CServiceTask.Migrations;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CServiceTask.Modules
{
    public class StartupModule : IFunctions, IReturnFunc
    {
        public void Return()
        {
            //CREATES A CAR SERVICE
            CreateCarService();

            //CREATES MECHANIC AND ADDS TO EXISTING SERVICE
            AddMechanicToService();

            //CREATES CLIENT AND ADDS TO EXISTING SERVICE
            CreateClientToService();

            //CREATES A MECHANIC AND CLIENT AND ADDS TO EXISTING SERVICE
            AddMechanicAndClientToService();

            //RETURNS ALL MECHANICS OF OF SELECTED SERVICE
            ReturnAllServiceMechanics();

            //RETURNS ALL CLIENTS OF OF SELECTED SERVICE
            ReturnAllServiceClients();

            //RETURNS ALL CLIENTS OF OF SELECTED MECHANIC
            ReturnAllClientsByMechanic();

            //UPDATES - CHANGES MECHANICS SERVICE TO CHOSEN ONE
            UpdateMechanicService();

            //UPDATES - MOVES MECHANIC TO SELECTED SERVICE AND GRABS CLIENTS FROM THE SELECTED SERVICE TO JOIN
            UpdateMechanicAndClients();
        }
        public void CreateCarService()
        {
            Console.WriteLine("How should your CarService be named? :");
            string name = Console.ReadLine();
            using (var dbContext = new carServiceContext())
            {
                dbContext.CarServices.Add(new CarService { Name = name });
                dbContext.SaveChanges();
            }
            Console.Clear();
            Console.WriteLine("CarService was created and added to the database!");
        }
        public void AddMechanicToService()
        {
            using var dbContext = new carServiceContext();

            bool continueAdding = true;
            while (continueAdding)
            {
                Console.WriteLine("Mechanic's name : ");
                string name = Console.ReadLine();

                Console.WriteLine("Job title : (Chassis mechanic, Engine mechanic, Body tuner, etc..)");
                string title = Console.ReadLine();

                Console.WriteLine("Pick service by its Id: ");
                var carServices = dbContext.CarServices.ToList();
                for (int i = 0; i < carServices.Count; i++)
                {
                    Console.WriteLine($"{carServices[i].Id} - {carServices[i].Name}");
                }
                string serviceId = Console.ReadLine();
                if (int.TryParse(serviceId, out int serviceIdInt))
                {
                    var service = carServices.FirstOrDefault(s => s.Id == serviceIdInt);
                    if (service != null)
                    {
                        dbContext.Mechanics.Add(new Mechanic(name, title) { CarServiceId = serviceIdInt });
                        dbContext.SaveChanges();
                        Console.Clear();
                        Console.WriteLine("Mechanic was successfully added to the database!");

                        bool continuePrompt = true;
                        while (continuePrompt)
                        {
                            Console.WriteLine("Add another mechanic? (y/n) :");
                            string userInput = Console.ReadLine().ToLower();
                            if (userInput == "y")
                            {
                                continuePrompt = false;
                            }
                            else if (userInput == "n")
                            {
                                continuePrompt = false;
                                continueAdding = false;
                            }
                            else
                            {
                                Console.WriteLine("Please enter 'y' or 'n'.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Service ID {serviceIdInt} does not exist in the database. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }
        public void CreateClientToService()
        {
            using var dbContext = new carServiceContext();
            while (true)
            {
                try
                {
                    Console.WriteLine("Please enter your first name:");
                    string firstName = Console.ReadLine();
                    if (string.IsNullOrEmpty(firstName))
                    {
                        throw new ArgumentException("First name cannot be empty");
                    }

                    Console.WriteLine("Please enter your last name:");
                    string lastName = Console.ReadLine();
                    if (string.IsNullOrEmpty(lastName))
                    {
                        throw new ArgumentException("Last name cannot be empty");
                    }

                    Console.WriteLine("Choose one of the following Car Services by its Id:");
                    var carServices = dbContext.CarServices.ToList();
                    for (int i = 0; i < carServices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1} - {carServices[i].Name}");
                    }

                    int serviceId;
                    while (!int.TryParse(Console.ReadLine(), out serviceId) || serviceId < 1 || serviceId > carServices.Count)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number between 1 and " + carServices.Count);
                    }
                    serviceId--;

                    var whereMechanicseqValue = dbContext.Mechanics.Where(x => x.CarServiceId == carServices[serviceId].Id).ToList();

                    Console.WriteLine("Choose one of the following Mechanics by its Id:");
                    for (int i = 0; i < whereMechanicseqValue.Count; i++)
                    {
                        Console.WriteLine($"{i + 1} - {whereMechanicseqValue[i].FirstName} - {whereMechanicseqValue[i].Title}");
                    }

                    int mechanicId;
                    while (!int.TryParse(Console.ReadLine(), out mechanicId) || mechanicId < 1 || mechanicId > whereMechanicseqValue.Count)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number between 1 and " + whereMechanicseqValue.Count);
                    }
                    mechanicId--;

                    List<Mechanic> mechanics = new List<Mechanic>();
                    Mechanic selectedMechanic = whereMechanicseqValue[mechanicId];
                    mechanics.Add(selectedMechanic);

                    dbContext.Clients.Add(new Client(firstName, lastName) { Mechanics = mechanics });
                    dbContext.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Client information was successfully transferred to the database!");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred. Please try again." + ex.Message);
                }
            }
        }
        public void AddMechanicAndClientToService()
        {
            using var dbContext = new carServiceContext();

            while (true)
            {
                try
                {
                    Console.WriteLine("Choose one of the following services by its Id:");
                    var carServices = dbContext.CarServices.ToList();
                    for (int i = 0; i < carServices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1} - {carServices[i].Name}");
                    }

                    int serviceId;
                    while (!int.TryParse(Console.ReadLine(), out serviceId) || serviceId < 1 || serviceId > carServices.Count)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number between 1 and " + carServices.Count);
                    }
                    serviceId--;

                    Console.WriteLine("Enter the first name of the mechanic you want to add:");
                    string firstName = Console.ReadLine();
                    if (string.IsNullOrEmpty(firstName))
                    {
                        throw new ArgumentException("First name cannot be empty");
                    }

                    Console.WriteLine("Enter the title of the mechanic:");
                    string title = Console.ReadLine();
                    if (string.IsNullOrEmpty(title))
                    {
                        throw new ArgumentException("Title cannot be empty");
                    }
                    Mechanic mechanic = new Mechanic(firstName, title) { CarServiceId = carServices[serviceId].Id };
                    List<Mechanic> Mechanics = new List<Mechanic>();
                    Mechanics.Add(mechanic);

                    dbContext.Mechanics.Add(mechanic);

                    Console.WriteLine("Enter the first name of the client you want to add:");
                    firstName = Console.ReadLine();
                    if (string.IsNullOrEmpty(firstName))
                    {
                        throw new ArgumentException("First name cannot be empty");
                    }

                    Console.WriteLine("Enter the last name of the client:");
                    string lastName = Console.ReadLine();
                    if (string.IsNullOrEmpty(lastName))
                    {
                        throw new ArgumentException("Last name cannot be empty");
                    }
                    List<CarService> ClientServices = new List<CarService>();
                    ClientServices.Add(carServices[serviceId]);

                    dbContext.Clients.Add(new Client(firstName, lastName) { Services = ClientServices, Mechanics = Mechanics });

                    dbContext.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("Mechanic and client were successfully added to the service!");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred. Please try again." + ex.Message);
                }
            }
        }
        public void UpdateMechanicService()
        {
            using var dbContext = new carServiceContext();

            bool continueUpdating = true;
            while (continueUpdating)
            {
                Console.WriteLine("Enter the Id of the mechanic you want to update: ");

                foreach(var mechanic in dbContext.Mechanics)
                {
                    Console.WriteLine($"ID [{mechanic.Id}] - Title {mechanic.Title} - Name {mechanic.FirstName}");
                }
                string mechanicId = Console.ReadLine();

                if (int.TryParse(mechanicId, out int mechanicIdInt))
                {
                    var mechanic = dbContext.Mechanics.FirstOrDefault(m => m.Id == mechanicIdInt);
                    if (mechanic != null)
                    {
                        Console.WriteLine("Pick new service by its Id: ");
                        var carServices = dbContext.CarServices.ToList();
                        for (int i = 0; i < carServices.Count; i++)
                        {
                            Console.WriteLine($"{carServices[i].Id} - {carServices[i].Name}");
                        }
                        string serviceId = Console.ReadLine();
                        if (int.TryParse(serviceId, out int serviceIdInt))
                        {
                            var service = carServices.FirstOrDefault(s => s.Id == serviceIdInt);
                            if (service != null)
                            {
                                mechanic.CarServiceId = serviceIdInt;
                                dbContext.SaveChanges();
                                Console.Clear();
                                Console.WriteLine("Mechanic's service was successfully updated!");

                                bool continuePrompt = true;
                                while (continuePrompt)
                                {
                                    Console.WriteLine("Update another mechanic's service? (y/n) :");
                                    string userInput = Console.ReadLine().ToLower();
                                    if (userInput == "y")
                                    {
                                        continuePrompt = false;
                                    }
                                    else if (userInput == "n")
                                    {
                                        continuePrompt = false;
                                        continueUpdating = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Please enter 'y' or 'n'.");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Service ID {serviceIdInt} does not exist in the database. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Mechanic ID {mechanicIdInt} does not exist in the database. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }

        public void UpdateMechanicAndClients()
        {
            using var dbContext = new carServiceContext();

            bool continueAdding = true;
            while (continueAdding)
            {
                Console.Clear();
                Console.WriteLine("Enter the ID of the mechanic you would like to update: ");

                foreach(var mechanic in dbContext.Mechanics)
                {
                    Console.WriteLine($"ID [{mechanic.Id}] - Title {mechanic.Title} - Name {mechanic.FirstName}");
                }
                string mechanicIdInput = Console.ReadLine();

                if (int.TryParse(mechanicIdInput, out int mechanicId))
                {
                    var mechanic = dbContext.Mechanics.Find(mechanicId);
                    if (mechanic != null)
                    {
                        Console.Clear();
                        Console.WriteLine("Enter the ID of the new CarService: ");

                        foreach(var service in dbContext.CarServices)
                        {
                            Console.WriteLine($"ID [{service.Id}] - Name {service.Name}");
                        }
                        string newServiceIdInput = Console.ReadLine();

                        if (int.TryParse(newServiceIdInput, out int newServiceId))
                        {
                            var newService = dbContext.CarServices.Find(newServiceId);
                            if (newService != null)
                            {
                                mechanic.CarServiceId = newServiceId;
                                dbContext.SaveChanges();

                                var clients = dbContext.Clients.Where(x => x.Services.Where(x => x.Id == newServiceId).Any());

                                var selectedClients = new List<Client>();
                                while (clients.Count() > 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Select clients to associate with the mechanic:");

                                    foreach(var client in clients)
                                    {
                                        Console.WriteLine($"ID - [{client.Id}] - Name {client.FirstName} {client.LastName}");
                                    }
                                    
                                    Console.WriteLine("Enter the ID of the client you would like to select or 'done' to finish: ");
                                    string selectedClientInput = Console.ReadLine();
                                    if (int.TryParse(selectedClientInput, out int selectedClientId))
                                    {
                                        var selectedClient = dbContext.Clients.Where(x => x.Id == selectedClientId).FirstOrDefault();
                                        if(selectedClient != null)
                                        {
                                            selectedClients.Add(selectedClient);
                                            var listClients = clients.ToList();
                                            listClients.Remove(selectedClient);
                                            clients = listClients.AsQueryable();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Client with that Id does not exist.");
                                        }
                                        
                                    }
                                    else if (selectedClientInput.ToLower() == "done")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input. Please enter a ID or 'done'.");
                                    }
                                }
                                dbContext.SaveChanges();
                                Console.Clear();
                                Console.WriteLine("The mechanic's CarService and clients have been successfully updated!");
                            }
                            else
                            {
                                Console.WriteLine($"CarService ID {newServiceId} does not exist in the database. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Mechanic ID {mechanicId} does not exist in the database. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                bool continuePrompt = true;
                while (continuePrompt)
                {
                    Console.WriteLine("Would you like to update another mechanic? (y/n) :");
                    string userInput = Console.ReadLine().ToLower();
                    if (userInput == "y")
                    {
                        continuePrompt = false;
                    }
                    else if (userInput == "n")
                    {
                        continuePrompt = false;
                        continueAdding = false;
                    }
                    else
                    {
                        Console.WriteLine("Please enter 'y' or 'n'.");
                    }
                }
            }
        }
        
        public void ReturnAllServiceMechanics()
        {
            using var dbContext = new carServiceContext();

            while (true)
            {
                try
                {
                    Console.WriteLine("All the data about Service Mechanics :");
                    Console.WriteLine("Enter your Id choice : ");
                    var serviceIds = dbContext.CarServices.Select(x => x.Id).ToList();
                    foreach (var service in dbContext.CarServices)
                    {
                        Console.WriteLine($"{service.Id} - {service.Name}");
                    }

                    int idToInt = 0;
                    while (!int.TryParse(Console.ReadLine(), out idToInt) || !serviceIds.Contains(idToInt))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number that exist in the list above.");
                    }

                    var retrieveMechanicsByCarServId = dbContext.Mechanics.Where(x => x.CarServiceId == idToInt).ToList();

                    if (retrieveMechanicsByCarServId.Count == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("No mechanics found for this service Id");
                        continue;
                    }

                    foreach (var mechanic in retrieveMechanicsByCarServId)
                    {
                        Console.WriteLine($"Id [{mechanic.Id}] - F-Name [{mechanic.FirstName}] - Title [{mechanic.Title}] ");
                    }
                    break;
                }
                catch (SqlException e)
                {
                    Console.WriteLine("An error occurred while retrieving the service mechanics from the database. Please try again later. " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred, please try again later." + e.Message);
                }
            }
        }
        public void ReturnAllServiceClients()
        {
            using var dbContext = new carServiceContext();
            while (true)
            {
                Console.WriteLine("All the data about Service Clients:");
                Console.WriteLine("Enter the ID of the service you want to see clients for: ");

                var services = dbContext.CarServices.ToList();
                services.ForEach(s => Console.WriteLine($"{s.Id} - {s.Name}"));

                var input = Console.ReadLine();
                if (!int.TryParse(input, out int serviceId))
                {
                    Console.WriteLine("Invalid input. Please enter a valid ID.");
                    continue;
                }

                if (!services.Any(s => s.Id == serviceId))
                {
                    Console.WriteLine("Service ID not found. Please enter a valid ID.");
                    continue;
                }

                var clients = dbContext.Clients.Where(c => c.Services.Any(s => s.Id == serviceId)).ToList();

                if (clients.Count() == 0)
                {
                    Console.Clear();
                    Console.WriteLine("No clients found for the selected service.");
                    continue;
                }
                else
                {
                    foreach (var client in clients)
                    {
                        Console.WriteLine($"ID [{client.Id}] - Name : {client.FirstName} {client.LastName}");
                    }
                }
                break;
            }
        }
        public void ReturnAllClientsByMechanic()
        {
            using var dbContext = new carServiceContext();
            while (true)
            {
                Console.WriteLine("Choose one of the following Mechanics by ID:");
                Console.WriteLine();

                var mechanics = dbContext.Mechanics.ToList();
                mechanics.ForEach(m => Console.WriteLine($"ID: {m.Id} | Service ID: {m.CarServiceId} | Name: {m.FirstName} {m.Title}"));

                var input = Console.ReadLine();
                if (!int.TryParse(input, out int mechanicId))
                {
                    Console.WriteLine("Invalid input. Please enter a valid ID.");
                    continue;
                }

                if (!mechanics.Any(m => m.Id == mechanicId))
                {
                    Console.WriteLine("Mechanic ID not found. Please enter a valid ID.");
                    continue;
                }
               
                var clients = dbContext.Clients.Where(x => x.Mechanics.Where(x => x.Id == mechanicId).Any());

                if (clients.Count() == 0)
                {
                    Console.Clear();
                    Console.WriteLine("No clients found for the selected mechanic.");
                    continue;
                }
                else
                {
                    foreach (var client in clients)
                    {
                        Console.WriteLine("Mechanics clients : ");
                        Console.WriteLine();
                        Console.WriteLine($"ID - [{client.Id}] - Name : {client.FirstName} {client.LastName}");
                        Console.WriteLine();
                    }
                }
                break;
            }
        }
    }
}
