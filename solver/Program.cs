using solver;
using solver.IO_Operations;

Console.Title = "Linear & Integer Programming Solver";

while (true)
{
    //Console.Clear();
    Console.WriteLine("==================================");
    Console.WriteLine("   Linear & Integer Programming");
    Console.WriteLine("==================================");
    Console.WriteLine("1. Load Model from Input File");
    Console.WriteLine("2. Solve with Primal Simplex");
    Console.WriteLine("3. Solve with Branch & Bound (Simplex)");
    Console.WriteLine("4. Solve with Cutting Plane Algorithm");
    Console.WriteLine("5. Solve with Branch & Bound Knapsack");
    Console.WriteLine("6. Sensitivity Analysis");
    Console.WriteLine("7. Exit");
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
            Algorithm.BranchAndBoundSimplex();
            break;
        case "4":
            Algorithm.CuttingPlane();
            break;
        case "5":
            Algorithm.KnapsackBranchAndBound();
            break;
        case "6":
            //SensitivityAnalysis sensitivityAnalysis = new SensitivityAnalysis();
            //SensitivityAnalysis.Menu();
            break;
        case "7":
            SensitivityAnalysis sensitivityAnalysis = new SensitivityAnalysis();
            sensitivityAnalysis.Menu();
            break;
        case "8":
            return; // Exit
        default:
            Console.WriteLine("Invalid choice. Press Enter to try again.");
            Console.ReadLine();
            break;
    }
}