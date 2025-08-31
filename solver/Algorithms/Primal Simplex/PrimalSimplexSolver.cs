using solver.IO_Operations;
using System.Text;

namespace solver.Algorithms.Primal_Simplex;

public class PrimalSimplexSolver
{
    protected double[,] tableau; // Table being processed at the time.
    protected int rows;
    protected int cols;
    protected int[] basis; // Values of all constraints, rhs and z rows.

    public PrimalSimplexSolver(double[,] initialTableau)
    {
        rows = initialTableau.GetLength(0);
        cols = initialTableau.GetLength(1);
        tableau = (double[,])initialTableau.Clone();

        // Basis starts with slack variables
        basis = new int[rows - 1];
        int numOriginalVars = cols - 1 - (rows - 1);
        for (int i = 0; i < basis.Length; i++)
        {
            basis[i] = numOriginalVars + i; // s1, s2, s3, etc.
        }
    }

    public double[,] SolveAndGetTableau()
    {
        Solve();
        return (double[,])tableau.Clone();
    }

    public void Solve()
    {
        Console.WriteLine("Initial Tableau:");
        PrintTableau(0);

        int iteration = 1;

        while (HasNegativeInObjectiveRow())
        {
            int pivotCol = FindPivotColumn();
            int pivotRow = FindPivotRow(pivotCol);

            if (pivotRow == -1)
            {
                Console.WriteLine("Problem is unbounded.");
                return;
            }

            Console.WriteLine($"\nIteration {iteration}:");
            Console.WriteLine($"Pivot column: {GetVariableName(pivotCol)} (column {pivotCol + 1})");
            Console.WriteLine($"Pivot row: {pivotRow + 1}");
            Console.WriteLine($"Pivot element: {tableau[pivotRow + 1, pivotCol]:F4}");

            UpdateBasis(pivotRow, pivotCol);
            PerformPivotOperation(pivotRow, pivotCol);

            PrintTableau(iteration);
            iteration++;
        }

        Console.WriteLine("\nOptimal Solution Found!");
        PrintFinalSolution();
    }

    protected bool HasNegativeInObjectiveRow()
    {
        for (int j = 0; j < cols - 1; j++) // Exclude RHS column
        {
            if (tableau[0, j] < 0)
                return true;
        }
        return false;
    }

    protected int FindPivotColumn()
    {
        int pivotCol = 0;
        double minValue = tableau[0, 0];

        for (int j = 1; j < cols - 1; j++) // Exclude RHS column
        {
            if (tableau[0, j] < minValue)
            {
                minValue = tableau[0, j];
                pivotCol = j;
            }
        }

        return pivotCol;
    }

    protected int FindPivotRow(int pivotCol)
    {
        int pivotRow = -1;
        double minRatio = double.MaxValue;

        for (int i = 1; i < rows; i++)
        {
            if (tableau[i, pivotCol] > 0)
            {
                double ratio = tableau[i, cols - 1] / tableau[i, pivotCol];
                if (ratio < minRatio && ratio >= 0)
                {
                    minRatio = ratio;
                    pivotRow = i - 1; // Adjust for 0-based basis indexing
                }
            }
        }

        return pivotRow;
    }

    protected void UpdateBasis(int pivotRow, int pivotCol)
    {
        basis[pivotRow] = pivotCol;
    }

    protected void PerformPivotOperation(int pivotRow, int pivotCol)
    {
        int actualPivotRow = pivotRow + 1; // Adjust for objective row

        // Get pivot element
        double pivotElement = tableau[actualPivotRow, pivotCol];

        // Normalize pivot row
        for (int j = 0; j < cols; j++)
        {
            tableau[actualPivotRow, j] /= pivotElement;
        }

        // Update other rows
        for (int i = 0; i < rows; i++)
        {
            if (i != actualPivotRow)
            {
                double factor = tableau[i, pivotCol];
                for (int j = 0; j < cols; j++)
                {
                    tableau[i, j] -= factor * tableau[actualPivotRow, j];
                }
            }
        }
    }

    protected void PrintTableau(int iteration)
    {
        Console.WriteLine($"\nTableau after iteration {iteration}:");
        Console.WriteLine("┌" + new string('─', 15 * (cols + 1) - 1) + "┐");

        // Header row
        Console.Write("│ Basis │");
        for (int j = 0; j < cols - 1; j++)
        {
            Console.Write($"{GetVariableName(j),12} │");
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
                Console.Write($"│ {GetVariableName(basis[i - 1]),5} │");
            }

            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{tableau[i, j],12:F4} │");
            }
            Console.WriteLine();

            if (i == 0)
            {
                Console.WriteLine("├" + new string('─', 7) + "┼" + new string('─', 14 * cols - 1) + "┤");
            }
        }

        Console.WriteLine("└" + new string('─', 7) + "┴" + new string('─', 14 * cols - 1) + "┘");
    }

    protected void PrintFinalSolution()
    {
        Console.WriteLine("\nFinal Solution:");
        double optimalValue = tableau[0, cols - 1];
        Console.WriteLine($"Optimal value: z = {optimalValue:F4}");

        double[] solution = GetSolution();
        int numOriginalVars = cols - 1 - (rows - 1);

        for (int i = 0; i < numOriginalVars; i++)
        {
            Console.WriteLine($"{GetVariableName(i)} = {solution[i]:F4}");
        }

        // Slack variables
        for (int i = numOriginalVars; i < cols - 1; i++)
        {
            double slackValue = 0;
            for (int j = 0; j < basis.Length; j++)
            {
                if (basis[j] == i)
                {
                    slackValue = tableau[j + 1, cols - 1];
                    break;
                }
            }
            Console.WriteLine($"{GetVariableName(i)} = {slackValue:F4}");
        }
    }

    protected string GetVariableName(int index)
    {
        int numOriginalVars = cols - 1 - (rows - 1);
        if (index < numOriginalVars) return $"x{index + 1}";
        if (index < cols - 1) return $"s{index - numOriginalVars + 1}";
        return "rhs";
    }

    public double[,] GetOptimalTableau()
    {
        return (double[,])tableau.Clone();
    }

    public int[] GetBasis()
    {
        return (int[])basis.Clone();
    }

    public double[] GetSolution()
    {
        double[] solution = new double[cols - 1]; // Exclude RHS column
        int numOriginalVars = cols - 1 - (rows - 1);

        for (int i = 0; i < basis.Length; i++)
        {
            if (basis[i] < numOriginalVars) // Only original variables
            {
                solution[basis[i]] = tableau[i + 1, cols - 1];
            }
        }
        return solution;
    }

    public double GetOptimalValue()
    {
        return tableau[0, cols - 1];
    }

    public bool IsOptimal()
    {
        return !HasNegativeInObjectiveRow();
    }

    public int GetNumberOfVariables()
    {
        return cols - 1 - (rows - 1);
    }

    public int GetNumberOfConstraints()
    {
        return rows - 1;
    }

    public static void PrimalSimplex(double[,] tableau)
    {
        PrimalSimplexSolver solver = new PrimalSimplexSolver(tableau);
        solver.Solve();
    }

    //Method that gets called to solve PrimalSimplex problem
    public static void PrimalSimplexAlgo(double[,] tableau)
    {
        PrimalSimplex(tableau);
    }

    //Method returns a solved optimal Table
    public static double[,] SolveAndReturnTableau(double[,] tableau)
    {
        PrimalSimplexSolver solver = new PrimalSimplexSolver(tableau);
        return solver.SolveAndGetTableau();
    }

    //Method serves as an example of ways to use this class
    public static void TestReturnOptTable()
    {
        double[,] initialTableau = new double[8, 14]
        {
                    // x1   x2   x3   x4   x5   x6   s1   s2   s3   s4   s5   s6   s7   rhs
                    { -2,  -3,  -3,  -5,  -2,  -4,   0,   0,   0,   0,   0,   0,   0,    0}, // Objective
                    { 11,   8,   6,  14,  10,  10,   1,   0,   0,   0,   0,   0,   0,   40}, // Constraint 1
                    {  1,   0,   0,   0,   0,   0,   0,   1,   0,   0,   0,   0,   0,    1}, // Constraint 2
                    {  0,   1,   0,   0,   0,   0,   0,   0,   1,   0,   0,   0,   0,    1}, // Constraint 3
                    {  0,   0,   1,   0,   0,   0,   0,   0,   0,   1,   0,   0,   0,    1}, // Constraint 4
                    {  0,   0,   0,   1,   0,   0,   0,   0,   0,   0,   1,   0,   0,    1}, // Constraint 5
                    {  0,   0,   0,   0,   1,   0,   0,   0,   0,   0,   0,   1,   0,    1}, // Constraint 6
                    {  0,   0,   0,   0,   0,   1,   0,   0,   0,   0,   0,   0,   1,    1}  // Constraint 7
        };

        // Method 1: Solve and get tableau
        PrimalSimplexSolver solver = new PrimalSimplexSolver(initialTableau);
        double[,] optimalTableau = solver.SolveAndGetTableau();

        // Method 2: Solve first, then get results
        solver.Solve();
        double[,] finalTableau = solver.GetOptimalTableau();
        int[] finalBasis = solver.GetBasis();
        double[] solution = solver.GetSolution();
        double optimalValue = solver.GetOptimalValue();

        // Method 3: Static method
        PrimalSimplexSolver.PrimalSimplex(initialTableau);

        // Method 4: Static method that returns tableau
        double[,] resultTableau = PrimalSimplexSolver.SolveAndReturnTableau(initialTableau);
    }

    public static void DisplayPrimalSimplex()
    {
        var (tableau, binaryVars) = IO.ParseFile();

        Console.WriteLine("Canonical Form Tableau:");
        IO.PrintTableau(tableau);

        Console.WriteLine($"\nBinary variables: {string.Join(", ", binaryVars.Select(v => $"x{v + 1}"))}");

        Console.WriteLine("Raw Tableau Array:");
        IO.PrintRawTableau(tableau);

        PrimalSimplexSolver.PrimalSimplex(tableau);
    }

    //Generates solution final solution
    public static void PrimalSimplexResult()
    {
        var (tableau, binaryVars) = IO.ParseFile();
        DisplayPrimalSimplex();
        PrimalSimplexAlgo(tableau);
    }

    // Method that returns a string of the result
    public static string GetResultAsString()
    {
        StringBuilder result = new StringBuilder();
        var originalOut = Console.Out;
        StringWriter stringWriter = new StringWriter();

        try
        {
            // Redirect console output to capture everything
            Console.SetOut(stringWriter);

            // Call the existing Solve method which prints to console
            PrimalSimplexResult(); //actual method or methods called to print result to console here.

            // Get all the captured output
            result.Append(stringWriter.ToString());
        }
        finally
        {
            // Restore console output
            Console.SetOut(originalOut);
        }

        return result.ToString();
    }
}
