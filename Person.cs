using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleAdmin
{
    internal class Person
    {
        // Declare values
        private string firstName;
        private string lastName;
        private string email;
        private string phoneNumber;
        private DateTime birthDay;

        // Set functions
        public void SetName(string firstName, string lastName)
        {
            this.firstName = firstName;
            this.lastName = lastName;
        }
        public void SetContact(string email, string phoneNumber)
        {
            this.email = email;
            this.phoneNumber = phoneNumber;
        }
        public void SetBirthDay(DateTime birthDay)
        {
            this.birthDay = birthDay;
        }

        // Get functions
        [JsonPropertyName("firstName")]
        public string FirstName { get { return this.firstName; } }
        [JsonPropertyName("lastName")]
        public string LastName { get { return this.lastName; } }
        [JsonPropertyName("email")]
        public string Email { get { return this.email; } }
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get { return this.phoneNumber; } }
        [JsonPropertyName("birthDay")]
        public DateTime BirthDay { get { return this.birthDay; } }
        [JsonIgnore]
        public int Age
        {
            get
            {
                TimeSpan diff = DateTime.Now - this.birthDay;
                int years = (int)(diff.TotalDays / 365.25);
                return years;
            }
        }

        public static explicit operator Dictionary<object, object>(Person v)
        {
            throw new NotImplementedException();
        }
    }
}