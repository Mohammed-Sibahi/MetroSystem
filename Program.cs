using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    class Station
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public Tuple<double, double> Coordinates { get; set; }
        public int Capacity { get; set; }
    }

    class Train
    {
        public int Number { get; set; }
        public int Capacity { get; set; }
    }

    class TicketType
    {
        public string Type { get; set; }
        public double InitialBalance { get; set; }
    }

    class Traveler
    {
        public int ID { get; set; }
        public Station CurrentStation { get; set; }
        public Station DestinationStation { get; set; }
    }

    static List<Station> stations = new List<Station>();
    static List<Train> trains = new List<Train>();
    static List<TicketType> ticketTypes = new List<TicketType>();
    static List<Traveler> travelers = new List<Traveler>();

    static void Main(string[] args)
    {
        InsertStations();
        InsertTrains();
        InsertTicketTypes();
        InsertTravelers();

        AssignTrainToStation();

      
       
        

        // Simulate travelers' journey
        AssignTravelersToStations();
        Console.WriteLine("Travelers assigned to stations...");

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine(); // Keeps the console open for viewing the output
    }

    static void InsertStations()
    {
        const int maxStations = 5;

        for (int stationCount = 0; stationCount < maxStations; stationCount++)
        {
            Console.Write("Enter station name (or 'done' to finish): ");
            string name = Console.ReadLine();
            if (name.ToLower() == "done")
                break;

            Console.Write("Enter station number: ");
            int number = int.Parse(Console.ReadLine());

            Console.Write("Enter X coordinate: ");
            double x = double.Parse(Console.ReadLine());

            Console.Write("Enter Y coordinate: ");
            double y = double.Parse(Console.ReadLine());

            Console.Write("Enter station capacity: ");
            int capacity = int.Parse(Console.ReadLine());

            Station station = new Station { Name = name, Number = number, Coordinates = Tuple.Create(x, y), Capacity = capacity };
            stations.Add(station);
        }
    }

    static void InsertTrains()
    {
        Console.Write("Enter the number of trains: ");
        int numTrains = int.Parse(Console.ReadLine());

        for (int i = 0; i < numTrains; i++)
        {
            Console.Write($"Enter train {i + 1} number: ");
            int number = int.Parse(Console.ReadLine());

            Console.Write($"Enter train {i + 1} capacity: ");
            int capacity = int.Parse(Console.ReadLine());

            Train train = new Train { Number = number, Capacity = capacity };
            trains.Add(train);
        }
    }

    static void InsertTicketTypes()
    {
        Console.Write("Enter the number of ticket types: ");
        int numTypes = int.Parse(Console.ReadLine());

        for (int i = 0; i < numTypes; i++)
        {
            Console.Write($"Enter ticket type {i + 1}: ");
            string ticketType = Console.ReadLine();

            Console.Write($"Enter initial balance for {ticketType}: ");
            double initialBalance = double.Parse(Console.ReadLine());

            TicketType ticket = new TicketType { Type = ticketType, InitialBalance = initialBalance };
            ticketTypes.Add(ticket);
        }
    }

    static void InsertTravelers()
    {
        Console.Write("Enter the number of travelers: ");
        int numTravelers = int.Parse(Console.ReadLine());

        for (int i = 0; i < numTravelers; i++)
        {
            Traveler traveler = new Traveler { ID = i + 1 };
            travelers.Add(traveler);
        }
    }

    static void AssignTrainToStation()
    {
        foreach (var train in trains)
        {
            Console.Write($"Assign train {train.Number} to station (enter station number): ");
            int stationNumber = int.Parse(Console.ReadLine());

            Station assignedStation = stations.Find(station => station.Number == stationNumber);

            Console.WriteLine($"Train {train.Number} assigned to station {assignedStation.Name}");
        }
    }

   


    static void AssignTravelersToStations()
    {
        foreach (var traveler in travelers)
        {
            Console.Write($"Assign traveler {traveler.ID} to station (enter station number): ");
            int stationNumber = int.Parse(Console.ReadLine());

            Station assignedStation = stations.Find(station => station.Number == stationNumber);

            traveler.CurrentStation = assignedStation;

            Console.Write($"Set destination station for traveler {traveler.ID} (enter station number): ");
            int destinationStationNumber = int.Parse(Console.ReadLine());

            Station destinationStation = stations.Find(station => station.Number == destinationStationNumber);

            traveler.DestinationStation = destinationStation;

            Console.WriteLine($"Traveler {traveler.ID} assigned to station {assignedStation.Name} with destination {destinationStation.Name}");

            // Simulate the traveler's journey with time based on destination
            SimulateTravelerJourney(traveler);
        }
    }

    static void SimulateTravelerJourney(Traveler traveler)
    {
        // Calculate journey time based on the difference in station numbers
        int journeyTimeInSeconds = Math.Abs(traveler.DestinationStation.Number - traveler.CurrentStation.Number) * 5;

        Console.WriteLine($"Traveler {traveler.ID} started their journey from {traveler.CurrentStation.Name} to {traveler.DestinationStation.Name}");

        for (int time = 1; time <= journeyTimeInSeconds; time++)
        {
            Console.WriteLine($"Time elapsed: {time}s");
            Thread.Sleep(1000); // Simulate the passage of time (1 second)

            // Check if the traveler has reached their destination
            if (time == journeyTimeInSeconds)
            {
                Console.WriteLine($"Traveler {traveler.ID} has reached their destination: {traveler.DestinationStation.Name}");
            }
        }
    }
}
