// CSV Helper
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using CsvHelper; // install-package CsvHelper
using CsvHelper.Configuration;

namespace PlayingAround
{
    class Program
    {
        private static readonly string _root = @"C:\";
        private static readonly string _csvPath = Path.Join(_root, "Users.csv");

        static void Main(string[] args)
        {
            var program = new Program();

            program.WriteSimpleFile();
            program.WriteMappedFile();
            foreach(var user in program.ReadUsersFile())
                Console.WriteLine("{0}. {1} {2} earns {3:C} and is{4} a UK resident", user.Id, user.Firstname, user.Surname, user.Salary, user.UkResident ? "" : "n't");

            File.Delete(_csvPath);
        }

        public void WriteSimpleFile()
        {
            using var writer = new StreamWriter(_csvPath, false);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            csv.WriteRecords(User.GetUsers());
        }

        public void WriteMappedFile()
        {
            using var writer = new StreamWriter(_csvPath, false);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            csv.Context.RegisterClassMap<UserMap>();
            csv.WriteRecords(User.GetUsers());
        }
        
        public List<User> ReadUsersFile()
        {
            using var reader = new StreamReader(_csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var users = new List<User>();

            csv.Read();
            csv.ReadHeader();
            
            while (csv.Read())
            {
                var user = new User(csv.GetField<int>("identifier"))
                {
                    Firstname = csv.GetField("firstname"),
                    Surname = csv.GetField("surname"),
                    DateOfBirth = DateTime.ParseExact(csv.GetField("date_of_birth"), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    Salary = csv.GetField<decimal>("salary"),
                    UkResident = csv.GetField("uk_reisdent").Equals("Y")
                };
                users.Add(user);
            }

            return users;
        }
    }

    public class User
    {
        private static int _id = 0;

        public int Id { get; private set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Salary { get; set; }
        public bool UkResident { get; set; }
        public bool IsRegistered { get; set; }

        public string DateOfBirthFormatted
        {
            get
            {
                return DateOfBirth.ToString("dd-MM-yyyy");
            }
        }

        public User(int? id = null)
        {
            if (id != null)
                Id = (int)id;
            else
                Id = ++_id;
        }

        public static List<User> GetUsers()
        {
            decimal salary = 1250.00M;
            return new List<User> // using System.IO;
            {
                new User { Firstname = "Foo", Surname = "Bar", DateOfBirth = new DateTime(1985, 4, 15), Salary = salary, UkResident = true },
                new User { Firstname = "A", Surname = "B", DateOfBirth = new DateTime(1996, 2, 13), Salary = salary += 4423.12M },
                new User { Firstname = "B", Surname = "C", DateOfBirth = new DateTime(1997, 5, 7), Salary = salary += 1002M, UkResident = true },
                new User { Firstname = "C", Surname = "D", DateOfBirth = new DateTime(1998, 7, 20), Salary = salary += 56.69M, UkResident = true },
                new User { Firstname = "D", Surname = "E", DateOfBirth = new DateTime(1999, 12, 31), Salary = salary += 2501.12M }
            };
        }
    }

    public class UserMap : ClassMap<User> // using CsvHelper.Configuration;
    {
        public UserMap()
        {
            //AutoMap(CultureInfo.InvariantCulture); // For if you only want to make a few alterations and the rest to be auto mapped

            Map(u => u.Id).Name("identifier");
            Map(u => u.Firstname).Name("firstname");
            Map(u => u.Surname).Name("surname");
            Map(u => u.DateOfBirth).Name("date_of_birth").Convert(u => u.DateOfBirth.ToString("dd-MM-yyyy")); // Inline type conversion
            Map(u => u.Salary).Name("salary");
            Map(u => u.UkResident).Name("uk_reisdent").Convert(u => u.UkResident ? "Y" : "N");

            Map(u => u.IsRegistered).Ignore();
        }
    }
}
