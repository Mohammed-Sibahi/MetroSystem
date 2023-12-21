using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; 

namespace DubizzleSystem
{
	class Program
	{
		// Represents a product in the system
		class Product
		{
			public int ID { get; set; }
			public string Name { get; set; }
			public decimal Price { get; set; }
			public string Category { get; set; }
			public bool IsSold { get; set; } = false;

			public override string ToString()
			{
				return $"{ID} - {Name} - ${Price} - Category: {Category}";
			}

			//public static Product FromString(string productAsString)
			//{
			//	var product = new Product();
			//	var productParts = productAsString.Split('-');
			//	product.ID = int.Parse(productParts[0].Trim());
			//	product.Name = productParts[1].Trim();
			//	product.Price = decimal.Parse(productParts[2].Trim().Substring(1));
			//	product.Category = productParts[3].Replace("Category:", "").Trim();

			//	return product;
			//}
		}

		// List to store products in the system
		static List<Product> products = new List<Product>();

		// Main entry point of the program
		static void Main()
		{
			Console.WriteLine("Welcome to the Dubbizle Console Clone!");

			// Keep inserting products until the user decides to stop
			if (File.Exists("products.txt"))
			{
				Console.WriteLine("I found saved products");
				//var productLines = File.ReadAllLines("products.txt");
				//foreach (var item in productLines)
				//{
				//	var product = Product.FromString(item);
				//	products.Add(product);
				//}

				var productsAsJson = File.ReadAllText("products.txt");
				products = JsonSerializer.Deserialize<List<Product>>(productsAsJson);

			}
			while (true)
			{

				Console.Write("Do you want to add another product? (y/n): ");
				string continueAdding = Console.ReadLine().ToLower();
				if (continueAdding != "y")
				{
					// Print products by category
					PrintProductsByCategory();

					// Allow the user to buy products
					BuyProducts();

					// Display farewell message
					Console.WriteLine("All products are out of stock. Please come back tomorrow.");
					Console.ReadLine();

					break;
				}
				else
					AddProduct();

			}
		}

		// Method to add a new product to the system
		static void AddProduct()
		{
			Product product = new Product();

			while (true)
			{
				Console.Write("Enter product ID: ");
				if (int.TryParse(Console.ReadLine(), out int id))
				{
					if (IsProductIdUnique(id))
					{
						product.ID = id;
						break;
					}
					else
					{
						Console.WriteLine("Product with the same ID already exists. Please enter a unique product ID.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid number for the product ID.");
				}
			}

			Console.Write("Enter product Name: ");
			product.Name = Console.ReadLine();

			Console.Write("Enter product Category: ");
			product.Category = Console.ReadLine();

			while (true)
			{
				Console.Write("Enter product Price: ");
				if (decimal.TryParse(Console.ReadLine(), out decimal price) && price > 0)
				{
					product.Price = price;
					break;
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid positive number for the product Price.");
				}
			}

			products.Add(product);
		}

		// Method to print products grouped by category
		static void PrintProductsByCategory()
		{
			var categories = products.Select(p => p.Category).Distinct();
			string allProductsAsString = JsonSerializer.Serialize(products);
			
			File.WriteAllText("products.txt", allProductsAsString);
			Console.WriteLine("Available products are:");

			foreach (var category in categories)
			{
				Console.WriteLine($"Category: {category}");

				// Print products under the same category
				foreach (var product in products.Where(p => p.Category == category && !p.IsSold))
				{
					Console.WriteLine(product);
				}

				Console.WriteLine(); // Add a line break after each category
			}
		}

		// Method to allow the user to buy products
		static void BuyProducts()
		{
			while (products.Any(p => !p.IsSold))
			{
				Console.Write("Enter the ID of the product you want to buy (or type 'exit' to stop): ");
				string input = Console.ReadLine();

				if (input.ToLower() == "exit")
					break;

				if (int.TryParse(input, out int selectedID))
				{
					var selectedProduct = products.FirstOrDefault(p => p.ID == selectedID && !p.IsSold);

					if (selectedProduct != null)
					{
						Console.WriteLine($"Do you want to buy {selectedProduct}? (yes/no)");
						string confirmation = Console.ReadLine().ToLower();

						if (confirmation == "yes")
						{
							// Ask for credit card information
							string creditCardInfo = GetCreditCardInfo();

							// Simulate processing the payment (in a real-world scenario, use a payment gateway)
							Console.WriteLine("Processing payment...");

							// Display the payment message
							Console.WriteLine("Payment processed successfully!");

							selectedProduct.IsSold = true;
						}
						else
						{
							Console.WriteLine($"You decided not to buy {selectedProduct}");
						}
					}
					else
					{
						Console.WriteLine("The product you are looking for is not available or has already been sold.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid product ID or type 'exit' to stop buying.");
				}

				// Print products by category after each purchase
				PrintProductsByCategory();
			}
		}

		// Method to check if a product ID is unique
		static bool IsProductIdUnique(int id)
		{
			return !products.Any(p => p.ID == id);
		}

		// Method to get valid credit card information from the user
		static string GetCreditCardInfo()
		{
			while (true)
			{
				Console.Write("Enter your credit card information (16 digits): ");
				string creditCardInfo = Console.ReadLine();
				string creditCardType = string.Empty;
				if (IsValidCreditCardInfo(creditCardInfo, out creditCardType))
				{
					Console.WriteLine(creditCardType);
					return creditCardInfo;
				}
				else
				{
					Console.WriteLine("Invalid credit card information. Please enter 16 digits within the specified range.");
				}
			}
		}

		// Method to check if credit card information is valid
		static bool IsValidCreditCardInfo(string creditCardInfo, out string cardType)
		{
			if (creditCardInfo.All(char.IsDigit) && creditCardInfo.Length == 16)
			{
				// Check if the credit card number falls within the specified range
				long creditCardNumber = long.Parse(creditCardInfo);
				long lowerBound = 1000000000000000;
				long upperBound = 7999999999999999;

				if (creditCardNumber >= lowerBound && creditCardNumber <= upperBound)
				{
					cardType = "Visa";
					return true;
				}
				else if (creditCardNumber >= 8000000000000000 && creditCardNumber <= 8999999999999999)
				{
					cardType = "MasterCard";
				}
				cardType = "Unknown";

				return creditCardNumber >= lowerBound && creditCardNumber <= upperBound;
			}

			cardType = "Unknown";
			return false;
		}
	}
}
