using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ConsoleAdmin
{
    internal class Functions
    {
        public bool exit = false;

        // Init variables
        private Person p = new Person();
        private string[] command;
        private string jsonPath = "data.json";
        private Dictionary<string, object> people = new Dictionary<string, object>();

        // Private functions
        // Static functions (do not edit data)
        // Help
        private void Help()
        {
            Console.WriteLine(
                "This application has the following commands (commands are not case sensitive):" +
                "\n\t1. Help, \n\t  Shows a list of commands that can be used in this application" +
                "\n\t2. View,\n\t  This can be used in different ways;\n\t    View / View all, which shows the full names of people without extra data\n\t    View {person full name (case sensitive)}; Shows all data from person specified" +
                "\n\t3. Add,\n\t  This will ask you to enter a persons information and save it to " + this.jsonPath +
                "\n\t4. Edit,\n\t  This will ask you to enter a person and edit its information" +
                "\n\t5. Delete,\n\t  This will delete a specified user and ask for confirmation" +
                "\n\t6. Exit,\n\t  Exit the program"
                );
        }
        // Viewing
        private void View()
        {
            string person = this.Identifier();

            if (person == "all" || person == "")
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
                        Console.WriteLine($"\t{i}. {details["firstName"]} {details["lastName"]}");

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
                // Load people
                Dictionary<string, object> data = this.Load();
                string personId = this.FetchPerson(this.Identifier());
                if (personId != null)
                {
                    // Write person data
                    Dictionary<string, object> details = JsonSerializer.Deserialize<Dictionary<string, object>>(data[personId].ToString());
                    Console.WriteLine(
                            $"{details["firstName"]} {details["lastName"]}" +
                            $"\n\tEmail: {details["email"]}" +
                            $"\n\tPhone number: {details["phoneNumber"]}" +
                            $"\n\tDate of birth: {details["birthDay"].ToString().Split('T')[0]}"
                            );
                }
            }
        }

        // Non static functions (changing data)
        // Adding people
        private void Add()
        {
            // Name
            Console.Write("First name: ");
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
            // Input validation
            if (!DateTime.TryParseExact(bday, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                Console.WriteLine($"'{bday}' is not a valid date in the format dd-mm-yyyy, please try again");
                this.Add();
                return;
            }

            this.p.SetBirthDay(parsedDate);

            // Confirm
            Console.WriteLine(
                    "\nYou have entered the following information, please confirm that this is correct:" +
                    $"\nFull name: {this.p.FirstName} {this.p.LastName}" +
                    $"\nEmail: {this.p.Email}" +
                    $"\nPhone: {this.p.PhoneNumber}" +
                    $"\nDate of birth: {this.p.BirthDay.ToString("dd-MM-yyyy")}" +
                    "\nPlease confirm by typing 'yes', 'no' or 'cancel'"
                );
            while (true) 
            {
                string save = Console.ReadLine().ToLower();
                if (save == "yes")
                {
                    // Add person to people and reset people to null
                    this.people[Guid.NewGuid().ToString()] = this.p;
                    this.Save();
                    this.p = new Person();
                    break;
                }
                else if (save == "no")
                {
                    this.Add();
                    break;
                }
                else if (save == "cancel")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("'" +  save + "' is not a valid answer");
                }
            }
        }
        private void Edit()
        {
            // Load people
            Dictionary<string, object> data = this.Load();
            string personId = this.FetchPerson(this.Identifier());
            if (personId != null)
            {
                Dictionary<string, object> newP = JsonSerializer.Deserialize<Dictionary<string, object>>(data[personId].ToString());
                Console.WriteLine($"Currently editing '{newP["firstName"]} {newP["lastName"]}'");

                // Name
                Console.Write("First name: ");
                newP["firstName"] = Console.ReadLine();
                Console.Write("Infix and last name: ");
                newP["lastName"] = Console.ReadLine();

                // Contact
                Console.Write("Email: ");
                newP["email"] = Console.ReadLine();
                Console.Write("Phone number: ");
                newP["phoneNumber"] = Console.ReadLine();

                // Birthday
                Console.Write("Date of birth (dd-mm-yyy): ");
                string bday = Console.ReadLine();
                // Input validation
                if (!DateTime.TryParseExact(bday, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    Console.WriteLine($"'{bday}' is not a valid date in the format dd-mm-yyyy, please try again");
                    this.Edit();
                    return;
                }
                newP["birthDay"] = parsedDate;

                // Check if everything is correct
                Console.WriteLine(
                    "\nYou have entered the following information, please confirm that this is correct:" +
                    $"\nFull name: {newP["firstName"]} {newP["lastName"]}" +
                    $"\nEmail: {newP["email"]}" +
                    $"\nPhone: {newP["phoneNumber"]}" +
                    $"\nDate of birth: {newP["birthDay"]}" +
                    "\nPlease confirm by typing 'yes', 'no' or 'cancel'"
                );

                while (true)
                {
                    string save = Console.ReadLine();
                    if (save == "yes")
                    {
                        data[personId] = JsonSerializer.Serialize(newP);
                        File.WriteAllText(this.jsonPath, JsonSerializer.Serialize(data));
                        break;
                    }
                    else if (save == "no")
                    {
                        this.Edit();
                        break;
                    }
                    else if (save == "cancel")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("'" + save + "' is not a valid answer");
                    }
                }          
            }
        }
        // Delete
        private void Delete()
        {
            try
            {
                // Get the name after command
                string name = this.Identifier();
                Dictionary<string, object> data = new Dictionary<string, object>();
                string personId = this.FetchPerson(name);

                if (personId != null)
                {
                    data = this.Load();
                    Dictionary<string, object> details = JsonSerializer.Deserialize<Dictionary<string, object>>(data[personId].ToString());

                    Console.WriteLine($"Are you sure you want to remove '{name}'? Please type 'yes' or 'no'");
                    string answer = Console.ReadLine().ToLower();
                    if (answer == "no")
                    {
                        // Chicken out when 'no is typed'
                        Console.WriteLine($"Canceled the removal of '{name}'");
                        return;
                    }

                    data.Remove(personId);
                }
                else
                {
                    return;
                }

                // Update the JSON
                string newJson = JsonSerializer.Serialize(data);
                File.WriteAllText(this.jsonPath, newJson);

                Console.WriteLine($"Successfully removed '{name}'");
            } 
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            
        }

        // Saving, loading and repeating functions
        private string Identifier()
        {
            // Get the name after command
            string name = "";
            for (int i = 0; i < command.Length; i++)
            {
                if (i != 0)
                {
                    name += command[i] + " ";
                }
            }

            if (name.Length > 1) 
            {
                return name.Remove(name.Length - 1, 1);
            }

            return name;
        }
        private string FetchPerson(string name)
        {
            // Load all data
            Dictionary<string, object> data = this.Load();
            foreach (var item in data)
            {
                // Brute force check items
                Dictionary<string, object> d = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Value.ToString());
                if ($"{d["firstName"]} {d["lastName"]}" == name)
                {
                    return item.Key;
                }
            }

            // Went thought the entire loop so nothing was found
            Console.WriteLine($"'{name}' was not found");
            return null;
        }
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
                case "help":
                    this.Help();
                    break;
                case "view":
                    this.View();
                    break;
                case "add":
                    this.Add();
                    break;
                case "edit":
                    this.Edit();
                    break;
                case "delete":
                    this.Delete();
                    break;
                case "exit":
                    this.Exit();
                    break;
                default:
                    Console.WriteLine("'" + this.command[0] + "' is not recognized as a command");
                    break;
            }
        }
    }
}
