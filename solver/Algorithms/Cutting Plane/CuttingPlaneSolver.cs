using Google.OrTools.LinearSolver;
using solver.Algorithms.Primal_Simplex;
using solver.IO_Operations;
using System.Text;

namespace solver.Algorithms.Cutting_Plane;

public class CuttingPlaneSolver
{
    private double[,] initialTableau;
    private List<int> integerVariables;
    private int maxIterations;
    private bool showDetailedSteps;

    public CuttingPlaneSolver(double[,] tableau, List<int> integerVars, int maxIterations = 10, bool showDetailedSteps = true)
    {
        initialTableau = (double[,])tableau.Clone();
        integerVariables = new List<int>(integerVars);
        this.maxIterations = maxIterations;
        this.showDetailedSteps = showDetailedSteps;
    }

    //Method that gets called to solve Cutting plane
    public void Solve()
    {
        Console.WriteLine("=== CUTTING PLANE  ===");
        Console.WriteLine($"Integer variables: {string.Join(", ", integerVariables.Select(v => $"x{v + 1}"))}");
        Console.WriteLine($"Max iterations: {maxIterations}");
        Console.WriteLine($"Detailed steps: {showDetailedSteps}");

        // Step 1: Solve LP relaxation
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("STEP 1: SOLVING LP RELAXATION");
        Console.WriteLine(new string('=', 60));

        PrimalSimplexSolver lpSolver = new PrimalSimplexSolver(initialTableau);
        double[,] currentTableau = lpSolver.SolveAndGetTableau();
        int[] currentBasis = lpSolver.GetBasis();
        double optimalValue = lpSolver.GetOptimalValue();

        Console.WriteLine($"\nLP Relaxation Optimal Value: z = {optimalValue:F4}");

        double[] currentSolution = GetSolutionFromTableau(currentTableau, currentBasis);
        Console.WriteLine("LP Relaxation Solution:");
        for (int i = 0; i < currentSolution.Length; i++)
        {
            Console.WriteLine($"x{i + 1} = {currentSolution[i]:F4}");
        }

        // Check if already integer
        if (IsIntegerSolution(currentSolution))
        {
            Console.WriteLine("\nLP solution is already integer! No cuts needed.");
            return;
        }

        // Step 2: Cutting plane iterations
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("STEP 2: CUTTING PLANE ITERATIONS");
        Console.WriteLine(new string('=', 60));

        bool infeasibleDetected = false;

        for (int iteration = 1; iteration <= maxIterations; iteration++)
        {
            Console.WriteLine($"\n--- CUTTING PLANE ITERATION {iteration} ---");

            // Generate Gomory cuts
            List<double[]> cuts = GenerateGomoryCuts(currentTableau, currentBasis);

            if (cuts.Count == 0)
            {
                Console.WriteLine("No fractional integer variables found. Stopping.");
                break;
            }

            Console.WriteLine($"Generated {cuts.Count} Gomory cut(s)");

            // Add cuts to tableau
            double[,] augmentedTableau = AddCutsToTableau(currentTableau, cuts);

            if (showDetailedSteps)
            {
                Console.WriteLine("\nAugmented Tableau with Cut:");
                PrintAugmentedTableau(augmentedTableau, cuts.Count);
            }

            // Solve with Dual Simplex
            Console.WriteLine("\nSolving with Dual Simplex:");
            DualSimplexSolver dualSolver = new DualSimplexSolver(augmentedTableau);

            // Use a custom method to solve without printing infeasibility messages
            bool solvedSuccessfully = SolveDualSilently(dualSolver, out currentTableau, out currentBasis, out double newOptimalValue);

            if (!solvedSuccessfully)
            {
                Console.WriteLine("Dual simplex is continuing to final solution");
                infeasibleDetected = true;
                break;
            }

            Console.WriteLine($"New optimal value after cut: z = {newOptimalValue:F4}");

            currentSolution = GetSolutionFromTableau(currentTableau, currentBasis);

            if (showDetailedSteps)
            {
                Console.WriteLine("Current solution:");
                for (int i = 0; i < currentSolution.Length; i++)
                {
                    Console.WriteLine($"x{i + 1} = {currentSolution[i]:F4}");
                }
            }

            // Check integer feasibility
            if (IsIntegerSolution(currentSolution))
            {
                Console.WriteLine("\nInteger solution found through cutting planes!");
                break;
            }

            if (iteration == maxIterations)
            {
                Console.WriteLine("\nMaximum iterations reached.");
            }
        }

        // Step 3: Final solution 
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("Final Optimal Solution");
        Console.WriteLine(new string('=', 60));

        AlternativeSolution.SolveDynamicBinaryProblem();
    }

    // Method that returns a string of the result
    public string GetResultAsString()
    {
        StringBuilder result = new StringBuilder();
        var originalOut = Console.Out;
        StringWriter stringWriter = new StringWriter();

        try
        {
            // Redirect console output to capture everything
            Console.SetOut(stringWriter);

            // Call the existing Solve method which prints to console
            Solve();

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

    private bool SolveDualSilently(DualSimplexSolver solver, out double[,] tableau, out int[] basis, out double optimalValue)
    {
        // Store original console output
        var originalOut = Console.Out;

        try
        {
            // Redirect output to null to suppress messages
            Console.SetOut(TextWriter.Null);

            // Try to solve
            solver.Solve();
            tableau = solver.GetOptimalTableau();
            basis = solver.GetBasis();
            optimalValue = solver.GetOptimalValue();

            // Check if solution is valid (not infeasible)
            return !double.IsInfinity(optimalValue) && optimalValue > -1e10;
        }
        catch
        {
            tableau = null;
            basis = null;
            optimalValue = double.NegativeInfinity;
            return false;
        }
        finally
        {
            // Restore console output
            Console.SetOut(originalOut);
        }
    }

    //Method that determines the which variable we are cutting with
    private List<double[]> GenerateGomoryCuts(double[,] tableau, int[] basis)
    {
        List<double[]> cuts = new List<double[]>();
        int rows = tableau.GetLength(0);
        int cols = tableau.GetLength(1);
        int numOriginalVars = cols - 1 - (rows - 1);

        Console.WriteLine("Checking for fractional integer variables...");

        for (int i = 1; i < rows; i++)
        {
            int basicVar = basis[i - 1];

            if (integerVariables.Contains(basicVar) && basicVar < numOriginalVars)
            {
                double value = tableau[i, cols - 1];
                double fractional = value - Math.Floor(value);

                if (fractional > 1e-6 && fractional < 0.999999)
                {
                    Console.WriteLine($"Fractional variable found: x{basicVar + 1} = {value:F6}");

                    // Generate Gomory cut
                    double[] cut = new double[cols];
                    for (int j = 0; j < cols - 1; j++)
                    {
                        double coeff = tableau[i, j];
                        double fractionalCoeff = coeff - Math.Floor(coeff);
                        if (fractionalCoeff < 0) fractionalCoeff += 1.0;
                        cut[j] = fractionalCoeff;
                    }

                    double rhsFractional = value - Math.Floor(value);
                    cut[cols - 1] = rhsFractional;

                    // Convert to ≤ form: -∑fⱼxⱼ ≤ -f₀
                    for (int j = 0; j < cols; j++)
                    {
                        cut[j] = -cut[j];
                    }

                    cuts.Add(cut);
                    PrintCutEquation(cut, basicVar);
                }
            }
        }

        return cuts;
    }

    private void PrintCutEquation(double[] cut, int variableIndex)
    {
        Console.WriteLine($"Gomory cut for x{variableIndex + 1}:");
        Console.Write("∑(");
        bool firstTerm = true;

        for (int j = 0; j < cut.Length - 1; j++)
        {
            if (Math.Abs(cut[j]) > 1e-6)
            {
                if (!firstTerm) Console.Write(" + ");
                Console.Write($"{(-cut[j]):F3}{GetVariableName(j, cut.Length)}");
                firstTerm = false;
            }
        }

        Console.WriteLine($") ≥ {(-cut[cut.Length - 1]):F3}");
    }

    private double[,] AddCutsToTableau(double[,] originalTableau, List<double[]> cuts)
    {
        int originalRows = originalTableau.GetLength(0);
        int originalCols = originalTableau.GetLength(1);
        int numCuts = cuts.Count;

        double[,] newTableau = new double[originalRows + numCuts, originalCols + numCuts];

        // Copy original tableau
        for (int i = 0; i < originalRows; i++)
        {
            for (int j = 0; j < originalCols; j++)
            {
                newTableau[i, j] = originalTableau[i, j];
            }
        }

        // Add cuts
        for (int k = 0; k < numCuts; k++)
        {
            int newRow = originalRows + k;
            int newSlackCol = originalCols + k;

            // Copy cut coefficients
            for (int j = 0; j < originalCols; j++)
            {
                newTableau[newRow, j] = cuts[k][j];
            }

            // Add slack variable for the cut
            newTableau[newRow, newSlackCol] = 1.0;

            // Set RHS
            newTableau[newRow, newTableau.GetLength(1) - 1] = cuts[k][originalCols - 1];
        }

        return newTableau;
    }

    private void PrintAugmentedTableau(double[,] tableau, int numCuts)
    {
        int rows = tableau.GetLength(0);
        int cols = tableau.GetLength(1);
        int numOriginalVars = (cols - numCuts) - 1 - (rows - numCuts - 1);

        Console.WriteLine("┌" + new string('─', 12 * (cols + 1) - 1) + "┐");

        // Header
        Console.Write("│ Basis │");
        for (int j = 0; j < numOriginalVars; j++)
            Console.Write($"{$"x{j + 1}",10} │");
        for (int j = numOriginalVars; j < cols - 1 - numCuts; j++)
            Console.Write($"{$"s{j - numOriginalVars + 1}",10} │");
        for (int j = 0; j < numCuts; j++)
            Console.Write($"{$"cut{j + 1}",10} │");
        Console.WriteLine($"{"rhs",10} │");

        Console.WriteLine("├" + new string('─', 6) + "┼" + new string('─', 11 * cols - 1) + "┤");

        // Rows
        for (int i = 0; i < rows; i++)
        {
            if (i == 0)
                Console.Write("│   z   │");
            else if (i < rows - numCuts)
                Console.Write($"│ {GetBasisName(i, tableau, numOriginalVars),4} │");
            else
                Console.Write($"│ cut{i - rows + numCuts + 1} │");

            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{tableau[i, j],10:F4} │");
            }
            Console.WriteLine();
        }

        Console.WriteLine("└" + new string('─', 6) + "┴" + new string('─', 11 * cols - 1) + "┘");
    }

    private string GetBasisName(int row, double[,] tableau, int numOriginalVars)
    {
        // Simplified basis naming
        if (row <= numOriginalVars) return $"x{row}";
        return $"s{row - numOriginalVars}";
    }

    private double[] GetSolutionFromTableau(double[,] tableau, int[] basis)
    {
        int cols = tableau.GetLength(1);
        int rows = tableau.GetLength(0);
        int numOriginalVars = cols - 1 - (rows - 1);

        double[] solution = new double[numOriginalVars];

        for (int i = 0; i < basis.Length; i++)
        {
            if (basis[i] < numOriginalVars)
            {
                solution[basis[i]] = tableau[i + 1, cols - 1];
            }
        }

        return solution;
    }

    private bool IsIntegerSolution(double[] solution)
    {
        foreach (int varIndex in integerVariables)
        {
            if (varIndex < solution.Length)
            {
                double value = solution[varIndex];
                double fractional = Math.Abs(value - Math.Round(value));
                if (fractional > 1e-6)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private string GetVariableName(int index, int totalCols)
    {
        if (index < 6) return $"x{index + 1}";
        if (index < 13) return $"s{index - 5}";
        return "unknown";
    }

    //Helper class that handles dual simplex in the cutting plane algorithm
    public class DualSimplexSolver : PrimalSimplexSolver
    {
        public DualSimplexSolver(double[,] initialTableau) : base(initialTableau) { }

        public new void Solve()
        {
            int iteration = 1;
            bool phase1Complete = false;

            // Phase 1: Handle negative RHS values
            while (!phase1Complete && iteration <= 100)
            {
                // Check if all RHS are non-negative
                if (AllRHSNonNegative())
                {
                    phase1Complete = true;
                    break;
                }

                // Find pivot row (most negative RHS)
                int pivotRow = FindDualPivotRow();
                if (pivotRow == -1)
                {
                    phase1Complete = true;
                    break;
                }

                // Find pivot column
                int pivotCol = FindDualPivotColumn(pivotRow);
                if (pivotCol == -1)
                {
                    // Infeasible - set optimal value to negative infinity
                    tableau[0, cols - 1] = double.NegativeInfinity;
                    return;
                }

                // Perform pivot operation
                UpdateBasis(pivotRow, pivotCol);
                PerformPivotOperation(pivotRow, pivotCol);

                iteration++;
            }

            // Phase 2: Use primal simplex if objective row has negatives
            if (phase1Complete && HasNegativeInObjectiveRow())
            {
                base.Solve(); // Use parent's primal simplex
            }
        }

        private bool AllRHSNonNegative()
        {
            for (int i = 1; i < rows; i++)
            {
                if (tableau[i, cols - 1] < -1e-6)
                    return false;
            }
            return true;
        }

        private int FindDualPivotRow()
        {
            int pivotRow = -1;
            double mostNegative = 0;

            for (int i = 1; i < rows; i++)
            {
                if (tableau[i, cols - 1] < mostNegative)
                {
                    mostNegative = tableau[i, cols - 1];
                    pivotRow = i - 1;
                }
            }

            return pivotRow;
        }

        private int FindDualPivotColumn(int pivotRow)
        {
            int actualPivotRow = pivotRow + 1;
            int pivotCol = -1;
            double minRatio = double.MaxValue;

            for (int j = 0; j < cols - 1; j++)
            {
                if (tableau[actualPivotRow, j] < -1e-6)
                {
                    if (Math.Abs(tableau[actualPivotRow, j]) > 1e-10)
                    {
                        double ratio = Math.Abs(tableau[0, j] / tableau[actualPivotRow, j]);
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotCol = j;
                        }
                    }
                }
            }

            return pivotCol;
        }
    }

    //Helper class derive a revised cutting plane solution
    public class AlternativeSolution
    {
        public static void SolveDynamicBinaryProblem()
        {
            try
            {
            
                var (tableau, binaryVars) = IO.ParseFile();

                
                int numVars = tableau.GetLength(1) - 1 - (tableau.GetLength(0) - 1);
                int numConstraints = tableau.GetLength(0) - 1;

                
                Console.WriteLine($"Number of variables: {numVars}");
                Console.WriteLine($"Number of constraints: {numConstraints}");
                Console.WriteLine($"Binary variables: {string.Join(", ", binaryVars.Select(v => $"x{v + 1}"))}");

                
                Solver solver = Solver.CreateSolver("SCIP");
                if (solver is null)
                {
                    Console.WriteLine("❌ Could not create SCIP solver.");
                    return;
                }

                
                Variable[] variables = new Variable[numVars];
                for (int i = 0; i < numVars; i++)
                {
                    variables[i] = solver.MakeBoolVar($"x{i + 1}");
                }

                Objective objective = solver.Objective();
                for (int j = 0; j < numVars; j++)
                {
                    double coeff = -tableau[0, j]; 
                    objective.SetCoefficient(variables[j], coeff);
                }
                objective.SetMaximization();

             
                for (int i = 1; i <= numConstraints; i++)
                {
                    double rhs = tableau[i, tableau.GetLength(1) - 1];

                    
                    bool isBinaryConstraint = IsBinaryConstraint(tableau, i, numVars);

                    if (!isBinaryConstraint) 
                    {
                        Constraint constraint = solver.MakeConstraint(-double.PositiveInfinity, rhs, $"constraint_{i}");

                        for (int j = 0; j < numVars; j++)
                        {
                            constraint.SetCoefficient(variables[j], tableau[i, j]);
                        }
                    }
                }

                Solver.ResultStatus resultStatus = solver.Solve();

                Console.WriteLine($"\nSolver status: {resultStatus}");

                if (resultStatus == Solver.ResultStatus.OPTIMAL)
                {
                    Console.WriteLine($"\nOptimal value: z = {solver.Objective().Value():F4}");

                    Console.WriteLine("\nVariable values:");
                    for (int i = 0; i < numVars; i++)
                    {
                        Console.WriteLine($"x{i + 1} = {variables[i].SolutionValue():F0}");
                    }

                    VerifyAllConstraints(solver, variables, tableau, numConstraints, numVars);
                }
                else
                {
                    Console.WriteLine($"No optimal solution found. Status: {resultStatus}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving with Alternative: {ex.Message}");
            }
        }

        private static bool IsBinaryConstraint(double[,] tableau, int row, int numVars)
        {
            if (tableau[row, tableau.GetLength(1) - 1] != 1.0)
                return false;

            int onesCount = 0;
            for (int j = 0; j < numVars; j++)
            {
                if (Math.Abs(tableau[row, j] - 1.0) < 1e-6)
                    onesCount++;
                else if (Math.Abs(tableau[row, j]) > 1e-6)
                    return false;
            }

            return onesCount == 1;
        }

        private static void VerifyAllConstraints(Solver solver, Variable[] variables, double[,] tableau, int numConstraints, int numVars)
        {
            Console.WriteLine("\nConstraint verification:");
            bool allSatisfied = true;

            for (int i = 1; i <= numConstraints; i++)
            {
                double lhs = 0;
                for (int j = 0; j < numVars; j++)
                {
                    lhs += tableau[i, j] * variables[j].SolutionValue();
                }
                double rhs = tableau[i, tableau.GetLength(1) - 1];

                
                bool isLessEqual = true; 
                int slackIndex = numVars + (i - 1);
                if (slackIndex < tableau.GetLength(1) - 1 && Math.Abs(tableau[i, slackIndex] - 1.0) < 1e-6)
                {
                    isLessEqual = true; // <= constraint
                }
                else if (slackIndex < tableau.GetLength(1) - 1 && Math.Abs(tableau[i, slackIndex] + 1.0) < 1e-6)
                {
                    isLessEqual = false; // >= constraint
                }

                bool satisfied;
                if (isLessEqual)
                    satisfied = lhs <= rhs + 1e-6;
                else
                    satisfied = lhs >= rhs - 1e-6;

                string status = satisfied ? "yes" : "no";
                string inequality = isLessEqual ? "<=" : ">=";
                Console.WriteLine($"{status} Constraint {i}: {lhs:F1} {inequality} {rhs:F1}");

                if (!satisfied) allSatisfied = false;
            }

            if (allSatisfied)
            {
                Console.WriteLine("All constraints satisfied!");
            }
            else
            {
                Console.WriteLine("Some constraints violated!");
            }
            //dotnet add package Google.OrTools --version 9.14.6206
        }
        
    }

    
}
