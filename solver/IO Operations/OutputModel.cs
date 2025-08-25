namespace solver;

public static class OutputModel
{
    public static void SaveOutput(string content)
    {
        File.WriteAllText("output.txt", content);
        Console.WriteLine("Results saved to output.txt");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
