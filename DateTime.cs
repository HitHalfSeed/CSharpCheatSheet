// DateTime
using System;
using System.Linq;
using System.Globalization;

namespace PlayingAround
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        public void Run()
        {
            Console.WriteLine(DateTime.Today);
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(DateTime.UtcNow);

            var birthdate = new DateTime(1990, 2, 23);
            var age = DateTime.Today - birthdate;

            Console.WriteLine(age);
            Console.WriteLine("You are {0} years old", age.Days / 365 );

            var d = new DateTime(2021, 2, 1);
            foreach (var i in Enumerable.Range(1, 12)) // using System.Linq;
            {
                var l = d.AddDays(-1);
                Console.WriteLine("The last day of {0} is {1} {2}", l.ToString("MMMM"), l.DayOfWeek, l.ToString("dd"));
                d = d.AddMonths(1);
            }

            var dateStrings = new string[] { "12/04/2021", "2021/11/07", "01012020", "55/12/2020" };
            foreach (var date in dateStrings)
            {
                DateTime result;
                if (DateTime.TryParseExact(date, new string[] { "dd/MM/yyyy", "yyyy/MM/dd", "ddMMyyyy" }, null, DateTimeStyles.None, out result))
                    Console.WriteLine("Success parsing date: " + result.ToString("yyyyMMdd"));
                else
                    Console.WriteLine("Error parsing date: " + date);
            }
        }
    }
}
