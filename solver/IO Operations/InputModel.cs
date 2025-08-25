using System;
using System.IO;

namespace solver.IO_Operations;

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
            Console.WriteLine("Model loaded successfully!");
        }
        else
        {
            Console.WriteLine("File not found!");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
