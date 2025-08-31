using System;
using System.IO;
using System.Linq;

namespace solver.IO_Operations
{
    public static class InputModel
    {
        public static void LoadModel()
        {
            Console.Write("Enter input file path: ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                ModelParser.Parse(lines);

                if (ModelParser.CurrentModel != null)
                {
                    Console.WriteLine("Model loaded successfully!");
                    PrintInitialTable(ModelParser.CurrentModel);
                }
                else
                {
                    Console.WriteLine("Failed to load model. Please check the input file format.");
                }
            }
            else
            {
                Console.WriteLine("File not found!");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static void PrintInitialTable(ProblemModel model)
        {
            Console.WriteLine("\n=== Initial Model Table ===");

            int n = model.ObjectiveCoefficients.Count;

            // Print header
            Console.Write("Obj | ");
            for (int i = 0; i < n; i++)
            {
                Console.Write($"x{i + 1}\t");
            }
            Console.WriteLine("RHS");

            // Print objective row
            Console.Write("Z   | ");
            foreach (var coeff in model.ObjectiveCoefficients)
            {
                Console.Write($"{coeff}\t");
            }
            Console.WriteLine("0"); // Objective RHS is 0

            // Print constraint row
            Console.Write("C1  | ");
            foreach (var coeff in model.ConstraintCoefficients)
            {
                Console.Write($"{coeff}\t");
            }
            Console.WriteLine(model.ConstraintRHS);

            // Print variable types
            Console.WriteLine("\nVariable Types:");
            for (int i = 0; i < model.VariableTypes.Count; i++)
            {
                Console.WriteLine($"x{i + 1}: {model.VariableTypes[i]}");
            }

            Console.WriteLine("===========================\n");
        }
    }
}
