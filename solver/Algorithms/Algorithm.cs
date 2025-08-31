using solver.Algorithms.Cutting_Plane;
using solver.Algorithms.Primal_Simplex;
using solver.IO_Operations;

namespace solver;

public static class Algorithm
{
    public static void PrimalSimplex()
    {
        Console.WriteLine("Running Primal Simplex...");
        PrimalSimplexSolver.PrimalSimplexResult();
        var result = PrimalSimplexSolver.GetResultAsString();

        OutputModel.SaveOutput(result);
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
        var results = solver.GetResultAsString();

        OutputModel.SaveOutput(results);
    }

    public static void KnapsackBranchAndBound()
    {
        solver.Algorithms.Knapsack.Algorithms.KnapsackBranchAndBound();
    }
}
