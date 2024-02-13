using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleAdmin
{
    internal class Functions
    {
        public bool exit = false;

        private Person p = new Person();
        private string[] command;
        private string jsonPath = "data.json";
        private Dictionary<string, object> people = new Dictionary<string, object>();

        // Private functions
        // Viewing
        private void View()
        {
            // Get the person to select
            string person = "";
            for (int i = 0; i < command.Length; i++)
            {
                if (i != 0)
                {
                    person += command[i] + " ";
                }
            }

            if (person == "all ")
            {
                // Load all
                Dictionary<string, object> people = this.Load();
                if (people != null)
                {
                    Console.WriteLine("Viewing all people. To view a persons details, please write 'view {person full name}'");

                    int i = 1;
                    foreach (var item in people)
                    {
                        // Get data
                        string jsonValue = item.Value.ToString();
                        Dictionary<string, object> details = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonValue);
                        Console.WriteLine($"{i}. {details["firstName"]} {details["lastName"]}");

                        i++;
                    }
                }
                else
                {
                    Console.WriteLine("There is nothing in this table");
                }
            }
            else
            {
                // Brute force check for the right person
                Dictionary<string, object> people = this.Load();
                bool found = false;

                foreach (var item in people)
                {
                    string jsonValue = item.Value.ToString();
                    Dictionary<string, object> details = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonValue);

                    if ($"{details["firstName"]} {details["lastName"]} " == person)
                    {
                        Console.WriteLine(
                            $"{details["firstName"]} {details["lastName"]}" +
                            $"\n\tEmail: {details["email"]}" +
                            $"\n\tPhone number: {details["phoneNumber"]}" +
                            $"\n\tDate of birth: {details["birthDay"].ToString().Split('T')[0]}"
                            );

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine($"'{person.Remove(person.Length - 1, 1)}' does not exist");
                }
            }
        }

        // Adding people
        private void Add()
        {
            // Name
            Console.Write("\nFirst name: ");
            string firstName = Console.ReadLine();
            Console.Write("Infix and last name: ");
            string lastName = Console.ReadLine();
            this.p.SetName(firstName, lastName);

            // Contact
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Phone number: ");
            string phone = Console.ReadLine();
            this.p.SetContact(email, phone);

            // Birthday
            Console.Write("Date of birth (dd-mm-yyy): ");
            string bday = Console.ReadLine();
            string[] bdaySplit = bday.Split('-');
            this.p.SetBirthDay(
                    new DateTime(
                        Convert.ToInt16(bdaySplit[2]),
                        Convert.ToInt16(bdaySplit[1]),
                        Convert.ToInt16(bdaySplit[0])
                    ));

            // Confirm
            Console.WriteLine(
                    "\nYou have entered the following information, please confirm that this is correct:" +
                    $"\nFull name: {this.p.FirstName} {this.p.LastName}" +
                    $"\nEmail: {this.p.Email}" +
                    $"\nPhone: {this.p.PhoneNumber}" +
                    $"\nDate of birth: {this.p.BirthDay.ToString("dd-MM-yyyy")}" +
                    "\nPlease confirm by typing 'yes' or 'no'"
                );
            while (true) 
            {
                string save = Console.ReadLine().ToLower();
                if (save == "yes")
                {
                    // Add person to people and reset people to null
                    this.people[Guid.NewGuid().ToString()] = this.p;
                    this.p = new Person();
                    break;
                }
                else if (save == "no")
                {
                    this.Add();
                    break;
                }
                else
                {
                    Console.WriteLine("'" +  save + "' is not a valid answer");
                }
            }
        }

        // Saving and loading
        private Dictionary<string, object> Load()
        {
            Dictionary<string, object> existing = new Dictionary<string, object>();

            if (File.Exists(this.jsonPath))
            {
                string existingJson = File.ReadAllText(this.jsonPath);

                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    existing = JsonSerializer.Deserialize<Dictionary<string, object>>(existingJson);
                }
            }

            return existing;
        }
        private void Save()
        {
            try
            {
                // Initialize existing data dictionary
                Dictionary<string, object> existing = this.Load();

                // Merge data
                foreach (var item in this.people)
                {
                    existing[item.Key] = item.Value;
                }

                // Write data
                string json = JsonSerializer.Serialize(existing, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(this.jsonPath, json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("An error occurred: " + e.Message);
            }
        }

        private void Exit()
        {
            this.Save();

            this.exit = true;
        }

        // Public functions
        public void GetCommand(string input)
        {
            string[] splitInput = input.Split(' ');
            this.command = splitInput;
        }

        public void HandleInput(string? altCom = null)
        {
            string commandLower = this.command[0].ToLower();
            if (altCom != null) { commandLower = altCom.ToLower(); }
           
            switch (commandLower)
            {
                case "view":
                    this.View();
                    break;
                case "add":
                    this.Add();
                    break;
                case "exit":
                    this.Exit();
                    break;
                default:
                    Console.WriteLine("'" + this.command[0] + "' is not recocnized as a command");
                    break;
            }
        }
    }
}
