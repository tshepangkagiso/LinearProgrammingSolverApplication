using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using solver.IO_Operations;

namespace solver.Algorithms.Knapsack
{
    public static class Algorithms
    {
        private struct Item
        {
            public int OriginalIndex;
            public int Value;
            public int Weight;
            public double Ratio;
        }

        private class Node
        {
            public int Level;
            public int Value;
            public int Weight;
            public double Bound;
            public bool[] Taken;
            public double[] FractionalSolution;
        }

        public static void KnapsackBranchAndBound()
        {
            var model = ModelParser.CurrentModel;
            if (model == null)
            {
                Console.WriteLine("No model loaded. Please load a problem first.");
                Console.ReadLine();
                return;
            }

            // --- Print initial table ---
            PrintInitialTable(model);

            int n = model.ObjectiveCoefficients.Count;
            if (n == 0 || model.ConstraintCoefficients.Count != n)
            {
                Console.WriteLine("Invalid model: objective/constraint length mismatch or empty.");
                return;
            }

            int capacity = model.ConstraintRHS;
            bool maximizing = model.IsMaximization;

            // Prepare items
            var items = new List<Item>(n);
            for (int i = 0; i < n; i++)
            {
                int value = model.ObjectiveCoefficients[i];
                if (!maximizing) value = -value;
                int weight = model.ConstraintCoefficients[i] > 0 ? model.ConstraintCoefficients[i] : 1;

                items.Add(new Item
                {
                    OriginalIndex = i,
                    Value = value,
                    Weight = weight,
                    Ratio = (double)value / weight
                });
            }

            items.Sort((a, b) => b.Ratio.CompareTo(a.Ratio));

            int bestValue = 0;
            bool[] bestTakenSorted = new bool[n];

            var root = new Node
            {
                Level = 0,
                Value = 0,
                Weight = 0,
                Taken = new bool[n],
                FractionalSolution = new double[n]
            };
            root.Bound = BoundFractional(items, root.Level, root.Value, root.Weight, capacity, root.FractionalSolution);

            var stack = new Stack<Node>();
            stack.Push(root);

            var sb = new StringBuilder();
            int subProblemCounter = 0;

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                // Print fractional solution
                Console.WriteLine($"Node Level {node.Level}, Value {node.Value}, Weight {node.Weight}, Bound {node.Bound:F2}");
                for (int i = 0; i < n; i++)
                {
                    Console.WriteLine($"\tx{i + 1} = {node.FractionalSolution[i]:0.##}");
                }
                if (node.Level > 0)
                    Console.WriteLine($"Sub-P {++subProblemCounter}: x{node.Level} = 0 / 1");

                Console.WriteLine("------------------------");
                sb.AppendLine($"Node Level {node.Level}, Value {node.Value}, Weight {node.Weight}, Bound {node.Bound:F2}");
                sb.AppendLine(string.Join("\t", node.FractionalSolution.Select(x => x.ToString("0.##"))));
                sb.AppendLine("------------------------");

                if (node.Level >= n)
                {
                    if (node.Value > bestValue)
                    {
                        bestValue = node.Value;
                        Array.Copy(node.Taken, bestTakenSorted, n);
                    }
                    continue;
                }

                var item = items[node.Level];

                // Include
                if (node.Weight + item.Weight <= capacity)
                {
                    var include = new Node
                    {
                        Level = node.Level + 1,
                        Value = node.Value + item.Value,
                        Weight = node.Weight + item.Weight,
                        Taken = (bool[])node.Taken.Clone(),
                        FractionalSolution = (double[])node.FractionalSolution.Clone()
                    };
                    include.Taken[node.Level] = true;
                    include.Bound = BoundFractional(items, include.Level, include.Value, include.Weight, capacity, include.FractionalSolution);
                    stack.Push(include);
                }

                // Exclude
                var exclude = new Node
                {
                    Level = node.Level + 1,
                    Value = node.Value,
                    Weight = node.Weight,
                    Taken = (bool[])node.Taken.Clone(),
                    FractionalSolution = (double[])node.FractionalSolution.Clone()
                };
                exclude.Taken[node.Level] = false;
                exclude.Bound = BoundFractional(items, exclude.Level, exclude.Value, exclude.Weight, capacity, exclude.FractionalSolution);
                stack.Push(exclude);
            }

            OutputModel.SaveOutput(sb.ToString());
        }

        private static double BoundFractional(List<Item> items, int level, int currentValue, int currentWeight, int capacity, double[] fractionalSolution)
        {
            double bound = currentValue;
            int weight = currentWeight;
            int n = items.Count;

            for (int i = 0; i < n; i++) fractionalSolution[i] = 0;

            for (int i = level; i < n; i++)
            {
                if (weight + items[i].Weight <= capacity)
                {
                    weight += items[i].Weight;
                    bound += items[i].Value;
                    fractionalSolution[i] = 1;
                }
                else
                {
                    int remain = capacity - weight;
                    fractionalSolution[i] = (double)remain / items[i].Weight;
                    bound += items[i].Ratio * remain;
                    break;
                }
            }
            return bound;
        }

        private static void PrintInitialTable(ProblemModel model)
        {
            Console.WriteLine("\n=== Initial Table ===");
            int n = model.ObjectiveCoefficients.Count;

            Console.Write("Obj | ");
            for (int i = 0; i < n; i++) Console.Write($"x{i + 1}\t");
            Console.WriteLine("RHS");

            Console.Write("Z   | ");
            foreach (var c in model.ObjectiveCoefficients) Console.Write($"{c}\t");
            Console.WriteLine("0");

            Console.Write("C1  | ");
            foreach (var c in model.ConstraintCoefficients) Console.Write($"{c}\t");
            Console.WriteLine(model.ConstraintRHS);

            Console.WriteLine("====================\n");
        }
    }
}
