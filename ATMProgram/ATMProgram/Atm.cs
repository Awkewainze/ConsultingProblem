using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ATMProgram
{
    public class Atm
    {
        private static readonly Dictionary<int, int> DefaultBillQuantities = new Dictionary<int, int>
        {
            { 1, 10 },
            { 5, 10 },
            { 10, 10 },
            { 20, 10 },
            { 50, 10 },
            { 100, 10 }
        };
        
        private static readonly Dictionary<int, int> CurrentBillQuantities = new Dictionary<int, int>(DefaultBillQuantities);

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input.Equals("Q", StringComparison.CurrentCultureIgnoreCase))
                    break;
                ParseConsoleInput(input);
            }
        }

        private static void ParseConsoleInput(string input)
        {
            input = input.Replace("$", "");
            if(input.Equals("R", StringComparison.CurrentCultureIgnoreCase))
            {
                RefillMachine();
            }
            else if (Regex.IsMatch(input, @"(?i)W\s+\d+"))
            {
                Withdraw(Regex.Match(input, @"\d+").Value);
            }
            else if (Regex.IsMatch(input, @"(?i)I(\s+\d+)+"))
            {
                DisplayBills(Regex.Match(input, @"\d+(\s\d+)*").Value);
            }
            else
            {
                Console.WriteLine("Invalid Command");
            }
        }

        private static void RemoveUsedBillsFromCurrent(Dictionary<int, int> bills)
        {
            foreach (var bill in bills)
            {
                CurrentBillQuantities[bill.Key] = CurrentBillQuantities[bill.Key] - bill.Value;
            }
        }

        private static void Withdraw(string amount)
        {
            Withdraw(int.Parse(amount));
        }

        private static void Withdraw(int amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Cannot withdraw zero or negative");
                return;
            }

            var billsUsed = new Dictionary<int, int>();
            var generatedValue = CurrentBillQuantities
                .OrderByDescending(x => x.Key)
                .Aggregate(0, (sum, x) =>
                              {
                                  var y = (amount - sum) / x.Key;
                                  if (y <= 0)
                                    return sum;

                                  if (y > x.Value)
                                      billsUsed[x.Key] = x.Value;
                                  else
                                      billsUsed[x.Key] = y;

                                  var sumOfCurrentBill = x.Key*billsUsed[x.Key];
                                  return sum + sumOfCurrentBill;
                              });

            if (generatedValue != amount)
            {
                Console.WriteLine("ERROR: Could not withdraw that amount\nPlease Contact Operator for Assistance");
                return;
            }

            RemoveUsedBillsFromCurrent(billsUsed);
            Console.WriteLine("Money withdrawn\nBills recieved: " + string.Join(", ", billsUsed));
            Console.WriteLine("Remaining Values");
            DisplayBills("1 5 10 20 50 100");
        }

        private static void DisplayBills(string values)
        {
            var x = values.Split(' ');
            foreach (var s in x)
            {
                if (!CurrentBillQuantities.ContainsKey(int.Parse(s)))
                {
                    Console.WriteLine(s + ": Not valid bill");
                    continue;
                }
                Console.Write($"${s}:\t");
                Console.WriteLine(CurrentBillQuantities[int.Parse(s)]);
            }
        }

        private static void RefillMachine()
        {
            Console.WriteLine("Enter nothing for default value");
            foreach (var bill in DefaultBillQuantities.Keys)
            {
                Console.Write($"${bill} ({DefaultBillQuantities[bill]}): ");
                var input = Console.ReadLine();
                if (input == "")
                    CurrentBillQuantities[bill] = DefaultBillQuantities[bill];
                else
                    CurrentBillQuantities[bill] = int.Parse(input);
            }
        }
    }
}
