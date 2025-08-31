using System;
using System.IO;

namespace solver.IO_Operations;

public static class InputModel
{
    public static void LoadModel()
    {
        Console.WriteLine("Please wait....");

        if (ModelParser.Parse())
        {
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
