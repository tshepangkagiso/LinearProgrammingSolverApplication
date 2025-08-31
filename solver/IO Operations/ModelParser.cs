using System;
using System.Collections.Generic;

namespace solver.IO_Operations
{
    public class ProblemModel
    {
        public bool IsMaximization { get; set; }
        public List<int> ObjectiveCoefficients { get; set; } = new();
        public List<int> ConstraintCoefficients { get; set; } = new();
        public int ConstraintRHS { get; set; }
        public List<string> VariableTypes { get; set; } = new();
    }

    public static class ModelParser
    {
        public static ProblemModel? CurrentModel { get; private set; }

        public static void Parse(string[] lines)
        {
            if (lines == null || lines.Length < 3)
            {
                Console.WriteLine("Invalid input format. Expected at least 3 lines (objective, constraint, var types).");
                return;
            }

            var model = new ProblemModel();

            // Line 1: Objective
            string[] objectiveParts = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (objectiveParts.Length == 0)
            {
                Console.WriteLine("Empty objective line.");
                return;
            }

            string verb = objectiveParts[0].Trim().ToLower();
            model.IsMaximization = verb == "max" || verb == "maximize" || verb == "maximization";

            for (int i = 1; i < objectiveParts.Length; i++)
            {
                string token = objectiveParts[i].Trim().Replace("+", "");
                if (int.TryParse(token, out int coeff))
                {
                    model.ObjectiveCoefficients.Add(coeff);
                }
                else
                {
                    Console.WriteLine($"Warning: couldn't parse objective coefficient '{objectiveParts[i]}'.");
                }
            }

            // Line 2: Constraint
            string[] constraintParts = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < constraintParts.Length; i++)
            {
                string t = constraintParts[i].Trim();
                if (t.Contains("<=") || t.Contains(">=") || t.Contains("="))
                {
                    string rhsStr = t.Replace("<=", "").Replace(">=", "").Replace("=", "").Trim();
                    if (rhsStr.Length > 0)
                    {
                        if (int.TryParse(rhsStr, out int rhsVal))
                            model.ConstraintRHS = rhsVal;
                    }
                    else if (i + 1 < constraintParts.Length && int.TryParse(constraintParts[i + 1], out int rhsVal2))
                        model.ConstraintRHS = rhsVal2;
                    break;
                }
                else
                {
                    string cleaned = t.Replace("+", "");
                    if (int.TryParse(cleaned, out int coeff))
                        model.ConstraintCoefficients.Add(coeff);
                }
            }

            // Line 3: Variable types
            string[] varTypes = lines[2].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            model.VariableTypes.AddRange(varTypes);

            CurrentModel = model;
        }
    }
}
