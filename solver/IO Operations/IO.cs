namespace solver.IO_Operations;

public class IO
{
    //Method that parses the entire file, returns Table and Binary Variables
    public static (double[,] tableau, List<int> binaryVars) ParseFile()
    {
        string filename = "input.txt";
        // Create file if it doesn't exist
        if (!File.Exists(filename))
        {
            CreateInputFile(filename);
            Console.WriteLine($"Created input file: {filename}");
        }

        var lines = File.ReadAllLines(filename);
        var binaryVars = new List<int>();
        var constraints = new List<double[]>();

        // Parse objective function
        var objectiveLine = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool isMax = objectiveLine[0].ToLower() == "max";
        var objectiveCoeffs = objectiveLine.Skip(1).Select(x => double.Parse(x.Trim('+'))).ToArray();

        // Parse constraints
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.ToLower().StartsWith("bin"))
            {
                // All variables are binary
                for (int j = 0; j < objectiveCoeffs.Length; j++)
                {
                    binaryVars.Add(j);
                }
            }
            else
            {
                // Parse regular constraint
                var constraintParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var coeffs = new List<double>();
                string inequality = "";
                double rhs = 0;
                bool foundInequality = false;

                foreach (var part in constraintParts)
                {
                    if (part == "<=" || part == ">=" || part == "=")
                    {
                        inequality = part;
                        foundInequality = true;
                    }
                    else if (double.TryParse(part.Trim('+'), out double num))
                    {
                        if (!foundInequality)
                            coeffs.Add(num);
                        else
                            rhs = num;
                    }
                }

                // Pad coefficients to match objective length
                while (coeffs.Count < objectiveCoeffs.Length)
                {
                    coeffs.Add(0);
                }

                constraints.Add(CreateConstraintRow(coeffs.ToArray(), inequality, rhs));
            }
        }

        // Add binary constraints: x_i <= 1 for each variable
        for (int i = 0; i < objectiveCoeffs.Length; i++)
        {
            var binaryConstraint = new double[objectiveCoeffs.Length];
            binaryConstraint[i] = 1; // Coefficient for x_i is 1
            constraints.Add(CreateConstraintRow(binaryConstraint, "<=", 1));
        }

        // Create tableau
        int numVars = objectiveCoeffs.Length;
        int numConstraints = constraints.Count;
        int numSlacks = numConstraints;

        double[,] tableau = new double[numConstraints + 1, numVars + numSlacks + 1];

        // Set objective row (negate for maximization)
        for (int j = 0; j < numVars; j++)
        {
            tableau[0, j] = isMax ? -objectiveCoeffs[j] : objectiveCoeffs[j];
        }

        // Set constraint rows
        for (int i = 0; i < numConstraints; i++)
        {
            for (int j = 0; j < numVars; j++)
            {
                tableau[i + 1, j] = constraints[i][j];
            }

            // Set RHS
            tableau[i + 1, numVars + numSlacks] = constraints[i][numVars];

            // Add slack variables (only for <= constraints)
            if (constraints[i][numVars + 1] == 1) // <= constraint
            {
                tableau[i + 1, numVars + i] = 1;
            }
        }

        return (tableau, binaryVars);
    }

    //Helper method to create binary constraints
    private static double[] CreateConstraintRow(double[] coeffs, string inequality, double rhs)
    {
        int slackType = 0;
        switch (inequality)
        {
            case "<=": slackType = 1; break;
            case ">=": slackType = -1; break;
        }

        var row = new double[coeffs.Length + 2];
        Array.Copy(coeffs, row, coeffs.Length);
        row[coeffs.Length] = rhs;
        row[coeffs.Length + 1] = slackType;

        return row;
    }

    //Method creates the LP problem if File doesnt exist
    private static void CreateInputFile(string filename)
    {
        string content = @"max +2 +3 +3 +5 +2 +4 
+11 +8 +6 +14 +10 +10 <= 40 
bin bin bin bin bin bin";

        File.WriteAllText(filename, content);
    }

    //Method prints out Table from input file
    public static void PrintTableau(double[,] tableau)
    {
        int rows = tableau.GetLength(0);
        int cols = tableau.GetLength(1);
        int numOriginalVars = (cols - 1) - (rows - 1); // Calculate original variables

        Console.WriteLine("┌" + new string('─', 15 * (cols + 1) - 1) + "┐");

        // Header row
        Console.Write("│ Basis │");
        for (int j = 0; j < numOriginalVars; j++)
        {
            Console.Write($"{$"x{j + 1}",12} │");
        }
        for (int j = numOriginalVars; j < cols - 1; j++)
        {
            Console.Write($"{$"s{j - numOriginalVars + 1}",12} │");
        }
        Console.WriteLine($"{"rhs",12} │");
        Console.WriteLine("├" + new string('─', 7) + "┼" + new string('─', 14 * cols - 1) + "┤");

        // Rows
        for (int i = 0; i < rows; i++)
        {
            if (i == 0)
            {
                Console.Write("│   z   │");
            }
            else
            {
                if (i <= numOriginalVars)
                    Console.Write($"│   s{i}   │");
                else
                    Console.Write($"│   s{i}   │");
            }

            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{tableau[i, j],12:F1} │");
            }
            Console.WriteLine();
        }

        Console.WriteLine("└" + new string('─', 7) + "┴" + new string('─', 14 * cols - 1) + "┘");
    }

    //Method prints out the raw Table without being parsed
    public static void PrintRawTableau(double[,] tableau)
    {
        int rows = tableau.GetLength(0);
        int cols = tableau.GetLength(1);

        Console.WriteLine("Raw Tableau Array:");
        Console.WriteLine("[");

        for (int i = 0; i < rows; i++)
        {
            Console.Write("  [");
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{tableau[i, j],8:F1}");
                if (j < cols - 1)
                    Console.Write(", ");
            }
            Console.WriteLine("],");
        }
        Console.WriteLine("]");
    }
}
