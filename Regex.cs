// Regex
using System;
using System.Text.RegularExpressions;

namespace PlayingAround
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            program.Ex1();
            program.Ex2();
            program.Ex3();
        }

        public void Ex1()
        {
            var name = "Foo Bar";

            var regex = new Regex("([a-z]*) ([a-z]*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (regex.IsMatch(name))
                Console.WriteLine("Name given is valid");

            var match = regex.Match(name);
            Console.WriteLine(match.Success); // true
            Console.WriteLine(match.Groups.Count); // 3

            foreach (var m in match.Groups)
                Console.WriteLine(m); // "Foo Bar", "Foo", "Bar"

            var firstname = match.Groups[1];
            var surname = match.Groups[2];

            Console.WriteLine("Hello {0} {1}", firstname, surname); // Hello Foo Bar
        }

        public void Ex2()
        {
            var input = "Random   ¬   s!'/entence  go==_+es he`|re";
            Console.WriteLine(input);
            input = Regex.Replace(input, @"[^a-z0-9\s]", "", RegexOptions.IgnoreCase);
            Console.WriteLine(input);
            input = Regex.Replace(input, @"\s+", " ");
            Console.WriteLine(input);
        }

        public void Ex3()
        {
            var fruits = "Apple, Pear, Pineapple, Plum";
            var fruitList = Regex.Split(fruits, @",\s+");
            foreach (var fruit in fruitList)
                Console.WriteLine("- '{0}'", fruit);
        }
    }
}