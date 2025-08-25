namespace solver;

public static class SensitivityAnalysis
{
    public static void Menu()
    {
        Console.Clear();
        Console.WriteLine("Sensitivity Analysis Options:");
        Console.WriteLine("1. Range of Non-Basic Variable");
        Console.WriteLine("2. Change Non-Basic Variable");
        Console.WriteLine("3. Range of Basic Variable");
        Console.WriteLine("4. Change Basic Variable");
        Console.WriteLine("5. Range of Constraint RHS");
        Console.WriteLine("6. Change Constraint RHS");
        Console.WriteLine("7. Add Activity");
        Console.WriteLine("8. Add Constraint");
        Console.WriteLine("9. Show Shadow Prices");
        Console.WriteLine("10. Duality Check");
        Console.WriteLine("0. Back");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Console.WriteLine("TODO: Show range of non-basic variable");
                break;
            case "9":
                Console.WriteLine("TODO: Display shadow prices");
                break;
            case "10":
                Console.WriteLine("TODO: Apply duality check");
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Feature not implemented yet.");
                break;
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
