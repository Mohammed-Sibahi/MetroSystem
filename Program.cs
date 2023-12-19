using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
	class Station
	{
		public string Name { get; set; }
		public int Number { get; set; }
		public Tuple<double, double> Coordinates { get; set; }
		public int Capacity { get; set; }

		public Train CurrentTrain { get; set; }

		public List<Traveler> Travelers { get; set; } = new List<Traveler>();
	}

	class Train
	{
		public int Number { get; set; }
		public int Capacity { get; set; }

		public Station CurrentStation { get; set; }

		public Station UpcomingStation { get; set; }
		public bool IsWaitingPassengers { get; set; } = false;
		public List<Traveler> Travelers { get; set; } = new List<Traveler>();

		private bool _isRunning = false;

		// Start of day
		public void Start(Station station, List<Station> stations)
		{
			CurrentStation = station;
			Console.WriteLine($"Train started from station {station.Name} '{station.Number}'");
			_isRunning = true;

			while (_isRunning)
			{
				Thread.Sleep(1000); // Simulate the passage of time (1 second)
				WaitPassengers(CurrentStation);
				Move(stations);
			}
		}

		public void WaitPassengers(Station station)
		{
			Console.WriteLine($"Train {Number} is waiting for passengers at {station.Name}...");
			IsWaitingPassengers = true;
			DropPassengers();
			Thread.Sleep(5000);
			IsWaitingPassengers = false;
		}

		public void DropPassengers()
		{
			foreach (var traveller in Travelers.ToArray())
			{
				if (traveller.Destination == CurrentStation)
				{
					Console.WriteLine($"Traveler {traveller.ID} has arrived at {traveller.Destination.Name} and dropped");
					Travelers.Remove(traveller);
					CurrentStation.Travelers.Add(traveller);
				}
			}
		}

		public void Move(List<Station> stations)
		{
			var index = stations.IndexOf(CurrentStation);
			if (index == stations.Count - 1)
				index = 0;
			else
				index++;
			UpcomingStation = stations[index];
			var currentStationName = CurrentStation.Name;
			CurrentStation.CurrentTrain = null;
			Console.WriteLine($"Train {Number} is moving from {CurrentStation.Name} to {UpcomingStation.Name}...");
			CurrentStation = null;

			for (int i = 0; i < 3; i++)
			{
				Console.WriteLine($"Train {Number} is moving from {currentStationName} to {UpcomingStation.Name}...");
				Thread.Sleep(1000);
			}
			Console.WriteLine($"Train {Number} has arrived at {UpcomingStation.Name}...");
			CurrentStation = UpcomingStation;
			UpcomingStation = null;
			CurrentStation.CurrentTrain = this;
		}


		// End of day
		public void Stop()
		{
			_isRunning = false;
			// TODO: Find the nearest station
		}
	}

	class TicketType
	{
		public string Type { get; set; }
		public double InitialBalance { get; set; }
	}

	class Traveler
	{
		public string ID { get; set; }

		public Traveler()
		{
			ID = Guid.NewGuid().ToString();
		}

		public Station Destination { get; set; }

		public void Travel(Station origin, Station destination)
		{
			Destination = destination;
			if (origin == destination)
			{
				Console.WriteLine($"Traveler {ID} is already at {origin.Name}");
				return;
			}
			while (origin.CurrentTrain == null)
			{
				Console.WriteLine($"Traveler {ID} is waiting for a train at {origin.Name}...");
				Thread.Sleep(1000);
			}
			var train = origin.CurrentTrain;

			for (int i = 0; i < 2; i++)
			{
				if (train.Travelers.Count < train.Capacity)
				{
					Console.WriteLine($"Traveler {ID} is getting on train {train.Number} at {origin.Name}");
					origin.CurrentTrain.Travelers.Add(this);
					origin.Travelers.Remove(this);
					return;
				}
				else
				{
					Console.WriteLine($"Train {train.Number} is full, traveler {ID} is waiting for travellers to drop");
				}
				Thread.Sleep(1000);
			}

			Console.WriteLine($"Train is full wait for the next one");
			Travel(origin, destination);
		}
	}

	static List<Station> stations = new List<Station>();
	static List<Train> trains = new List<Train>();
	static List<TicketType> ticketTypes = new List<TicketType>();
	static List<Traveler> travelers = new List<Traveler>();

	static async Task Main(string[] args)
	{
		InsertStations();
		InsertTrains(stations);

		int index = 0;

		var traveler = new Traveler();
		stations[0].Travelers.Add(traveler);
		var first = Task.Run(() =>
		{
			trains[0].Start(stations[1], stations);
		});
		var second = Task.Run(() =>
		{
			traveler.Travel(stations[0], stations[1]);
		});

		await Task.WhenAll(first, second);
		
		// Assign stations to trains 


		//InsertTicketTypes();
		//InsertTravelers();

		//AssignTrainToStation();





		//// Simulate travelers' journey
		//AssignTravelersToStations();
		//Console.WriteLine("Travelers assigned to stations...");

		//Console.WriteLine("Press enter to exit.");
		//Console.ReadLine(); // Keeps the console open for viewing the output
	}

	static void InsertStations()
	{
		var input = string.Empty;
		while (input != "done")
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
			Console.WriteLine("Station added successfully! Add more? or insert 'done' to finish");
			input = Console.ReadLine();
		}

	}

	static void InsertTrains(List<Station> stations)
	{
		Console.Write("Enter the number of trains: ");
		int numTrains = int.Parse(Console.ReadLine());
		if (numTrains > stations.Count)
		{
			Console.WriteLine("Number of trains can't be more than number of stations");
			numTrains = stations.Count;
		}
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

			//traveler.CurrentStation = assignedStation;

			Console.Write($"Set destination station for traveler {traveler.ID} (enter station number): ");
			int destinationStationNumber = int.Parse(Console.ReadLine());

			Station destinationStation = stations.Find(station => station.Number == destinationStationNumber);

			//traveler.DestinationStation = destinationStation;

			Console.WriteLine($"Traveler {traveler.ID} assigned to station {assignedStation.Name} with destination {destinationStation.Name}");

			// Simulate the traveler's journey with time based on destination
			SimulateTravelerJourney(traveler);
		}
	}

	static void SimulateTravelerJourney(Traveler traveler)
	{
		// Calculate journey time based on the difference in station numbers
		//int journeyTimeInSeconds = Math.Abs(traveler.DestinationStation.Number - traveler.CurrentStation.Number) * 5;

		//Console.WriteLine($"Traveler {traveler.ID} started their journey from {traveler.CurrentStation.Name} to {traveler.DestinationStation.Name}");

		//for (int time = 1; time <= journeyTimeInSeconds; time++)
		//{
		//    Console.WriteLine($"Time elapsed: {time}s");
		//    Thread.Sleep(1000); // Simulate the passage of time (1 second)

		//    // Check if the traveler has reached their destination
		//    if (time == journeyTimeInSeconds)
		//    {
		//        Console.WriteLine($"Traveler {traveler.ID} has reached their destination: {traveler.DestinationStation.Name}");
		//    }
		//}
	}
}
