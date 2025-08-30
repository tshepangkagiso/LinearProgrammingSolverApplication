using solver.Algorithms.Cutting_Plane;
using solver.Algorithms.Primal_Simplex;
using solver.IO_Operations;

namespace solver;

public static class Algorithm
{
    public static void PrimalSimplex()
    {
        Console.WriteLine("Running Primal Simplex...");

        var (tableau, binaryVars) = IO.ParseFile();
        PrimalSimplexSolver.DisplayPrimalSimplex();
        PrimalSimplexSolver.PrimalSimplexAlgo(tableau);

        OutputModel.SaveOutput("Primal Simplex results here...");
    }

    public static void RevisedSimplex()
    {
        Console.WriteLine("Running Revised Simplex...");
        // TODO
        OutputModel.SaveOutput("Revised Simplex results here...");
    }

    public static void BranchAndBoundSimplex()
    {
        Console.WriteLine("Running Branch & Bound Simplex...");
        // TODO
        OutputModel.SaveOutput("Branch & Bound Simplex results here...");
    }

    public static void CuttingPlane()
    {
        var (tableau, binaryVars) = IO.ParseFile();
        CuttingPlaneSolver solver = new CuttingPlaneSolver(tableau, binaryVars, maxIterations: 10, showDetailedSteps: true);
        solver.Solve();

        OutputModel.SaveOutput("Cutting Plane results here...");
    }

    public static void KnapsackBranchAndBound()
    {
        Console.WriteLine("Running Branch & Bound Knapsack...");
        // TODO
        OutputModel.SaveOutput("Knapsack Branch & Bound results here...");
    }


}
