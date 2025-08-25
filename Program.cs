using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//input file containing LP
//process LP and get solution
//output new file with solution

namespace Solver
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Linear & Integer Programming Solver";

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==================================");
                Console.WriteLine("   Linear & Integer Programming");
                Console.WriteLine("==================================");
                Console.WriteLine("1. Load Model from Input File");
                Console.WriteLine("2. Solve with Primal Simplex");
                Console.WriteLine("3. Solve with Revised Simplex");
                Console.WriteLine("4. Solve with Branch & Bound (Simplex)");
                Console.WriteLine("5. Solve with Cutting Plane Algorithm");
                Console.WriteLine("6. Solve with Branch & Bound Knapsack");
                Console.WriteLine("7. Sensitivity Analysis");
                Console.WriteLine("8. Exit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        InputModel.LoadModel();
                        break;
                    case "2":
                        Algorithm.PrimalSimplex();
                        break;
                    case "3":
                        Algorithm.RevisedSimplex();
                        break;
                    case "4":
                        Algorithm.BranchAndBoundSimplex();
                        break;
                    case "5":
                        Algorithm.CuttingPlane();
                        break;
                    case "6":
                        Algorithm.KnapsackBranchAndBound();
                        break;
                    case "7":
                        SensitivityAnalysis.Menu();
                        break;
                    case "8":
                        return; // Exit
                    default:
                        Console.WriteLine("Invalid choice. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
