// Array Indices
using System;

namespace PlayingAround
{
    class Program
    {
        static void Main(string[] args)
        {
            Ex1();
        }

        public static void Ex1()
        {
            var numbers = new int[20];

            for (var i = 0; i < numbers.Length; i++)
                numbers[i] = i;

            Console.WriteLine("First ten elements...");
            foreach (var j in numbers[0..10]) 
                Console.WriteLine(j); // 0,1,2,3,4,5,6,7,8,9

            Console.WriteLine("{0}Tenth element onwards", Environment.NewLine);
            foreach (var i in numbers[10..]) // 10,11,12,13,14,15,16,17,18,19
                Console.WriteLine(i);

            Console.WriteLine("{0}First 7 elements", Environment.NewLine);
            foreach (var i in numbers[..7]) // 0,1,2,3,4,5,6
                Console.WriteLine(i);

            Console.WriteLine("{0}Last element", Environment.NewLine);
            Console.WriteLine(numbers[^1]); // 19

            Console.WriteLine("{0}Third last element", Environment.NewLine);
            Console.WriteLine(numbers[^3]); // 17

            Console.WriteLine("{0}15th last element through to 12th last element", Environment.NewLine);
            foreach (var i in numbers[^15..^12])
                Console.WriteLine(i); // 6,5,7
        }
    }
}
