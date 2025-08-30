using System.Data;
using System.Numerics;

namespace solver;

public class SensitivityAnalysis
{
    private string goal = "max";
    private List<Table> tables = new List<Table>();

    private Table table = new Table(new double[,]
    {
            { -3, -2, 0, 0, 0, 0 },
            {  2,  1, 1, 0, 0, 100 },
            {  1,  1, 0, 1, 0, 80 },
            {  1,  0, 0, 0, 1, 40 }
    });

    private Table table2 = new Table(new double[,]
    {
            { 0, -2, 0, 0, 3, 120 },
            {  0,  1, 1, 0, -2, 20 },
            {  0,  1, 0, 1, -1, 40 },
            {  1,  0, 0, 0, 1, 40 }
    });

    private Table table3 = new Table(new double[,]
    {
            { 0, 0, 2, 0, -1, 160 },
            {  0,  1, 1, 0, -2, 20 },
            {  0,  0, -1, 1, 1, 20 },
            {  1,  0, 0, 0, 1, 40 }
    });

    private Table table4 = new Table(new double[,]
    {
            { 0, 0, 1, 1, 0, 180 },
            {  0,  1, -1, 2,0, 60 },
            {  0,  0, -1, 1, 1, 20 },
            {  1,  0, 1, -1, 0, 20 }
    });
    public void Menu()
    {
        tables.Add(table);
        tables.Add(table2);
        tables.Add(table3);
        tables.Add(table4);
        Console.Clear();
        Console.WriteLine("Sensitivity Analysis Options:");//working
        Console.WriteLine("1. Range of Non-Basic Variable");//working
        Console.WriteLine("2. Change Non-Basic Variable");//TODO
        Console.WriteLine("3. Range of Basic Variable");//working
        Console.WriteLine("4. Change Basic Variable");//TODO
        Console.WriteLine("5. Range of Constraint RHS");//Fix
        Console.WriteLine("6. Change Constraint RHS");//TODO
        Console.WriteLine("7. Add Activity");//fix
        Console.WriteLine("8. Add Constraint");//working - finish w pivoting
        Console.WriteLine("9. Show Shadow Prices");//fix
        Console.WriteLine("10. Duality Check");//TODO
        Console.WriteLine("0. Back");

        string choice = Console.ReadLine();
        while (choice != "0")
        {
            switch (choice)
            {
                case "1"://Range of NBV
                    {
                        Console.WriteLine("====================================================================");
                        Console.WriteLine("Please enter the index of the value you want to find the range of (0-?)");
                        int index = int.Parse(Console.ReadLine()); //Do some error handling later on
                        DisplayNonBasicVariableRanges(tables[tables.Count - 1], index);//Passing it the optimal table, and the ColIndex of the range to be found
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        Console.ReadLine();
                        choice = "11";
                        break;
                    }
                case "2"://Display change in NBV range
                    {
                        //This involves pivoting
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        choice = "11";
                        break;
                    }
                case "3"://Range of BV
                    {
                        Console.WriteLine("====================================================================");
                        Console.WriteLine("Please enter the index of the value you want to find the range of (0-?)");
                        int index = int.Parse(Console.ReadLine());
                        DisplayBasicVariableRange(tables.Last(), index);
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        Console.ReadKey();
                        choice = "11";
                        break;
                    }
                case "4"://Display change in range of BV
                    {
                        //This involves pivoting
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        choice = "11";
                        break;
                    }
                case "5"://Range of RHS
                    {
                        Console.WriteLine("====================================================================");
                        Console.WriteLine("Please enter the row number that you want to see the RHS range for");
                        int row = int.Parse(Console.ReadLine());
                        Console.WriteLine();
                        DisplayRHSRange(tables.Last(), tables.First(), row);
                        Console.ReadKey();
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        choice = "11";
                        break;
                    }
                case "6"://Display change in range of RHS
                    {
                        //This involves pivoting
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        choice = "11";
                        break;
                    }
                case "7"://Add activity - Come back to this one
                    {
                        Console.WriteLine("====================================================================");
                        //AddNewActivity(Table optimalTable, double[] newColumn, double newObjectiveCoeff, bool isMaximization)
                        Console.WriteLine("Please enter the objective function (z) value of the new activity");
                        double objValue = double.Parse(Console.ReadLine());
                        Console.WriteLine("Please enter the constraint values of the new activity. Use a space to split and add signs (+x +x -y -y +x -y)");
                        string constraints = Console.ReadLine();
                        string[] numerals = constraints.Split(" ");
                        double[] column = new double[numerals.Length];
                        int index = 0;
                        foreach(var num in numerals)
                        {
                            column[index] = double.Parse(num);
                            index++;
                        }
                        Console.WriteLine("====================================================================");
                        AddNewActivity(tables.Last(), column, objValue, (goal.ToLower() == "max") ? true: false);
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        Console.ReadKey();
                        choice = "11";
                        break;
                    }
                case "8"://Add constraint - This works, but might need to add pivoting
                    {
                        Console.WriteLine("====================================================================");
                        //AddNewConstraint(Table optimalTable, double[] newConstraint, double rhs)
                        Console.WriteLine("Please enter the column values of the new constriant. Use a space to split and add signs (+x +x -y -y +x -y). Do not include RHS value");
                        string constraints = Console.ReadLine();
                        string[] numerals = constraints.Split(" ");
                        double[] row = new double[numerals.Length];
                        int index = 0;
                        foreach (var num in numerals)
                        {
                            row[index] = double.Parse(num);
                            index++;
                        }
                        Console.WriteLine("Please input the RHS value of the new constraint");
                        double RHS = double.Parse(Console.ReadLine());
                        Console.WriteLine(AddNewConstraint(tables.Last(), row, RHS));
                        Console.ReadKey();
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        choice = "11";
                        break;
                    }
                case "9"://Shadow prices
                    {
                        Console.WriteLine("====================================================================");
                        //public double[] ComputeShadowPrices(Table optimalTable)
                        int counter = 0;
                        double[] shadowPrices = ComputeShadowPrices(tables.Last());
                        foreach (var shadowPrice in shadowPrices)
                        {
                            Console.WriteLine($"Shadow price for variable at index {++counter}: R{shadowPrice:0.00}");
                        }

                        Console.ReadKey();
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        choice = "11";
                        break;
                    }

                case "10":
                    {
                        Console.WriteLine("====================================================================");
                        Console.WriteLine("Please enter the index of the value you want to find the range of (0-?)");
                        int index = int.Parse(Console.ReadLine());
                        DisplayBasicVariableRange(tables.Last(), index);
                        Console.WriteLine();
                        Console.WriteLine("Please press any button to return to the main screen");
                        choice = "11";
                        break;
                    }
                case "11":
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
                        choice = Console.ReadLine();
                        break;
                    }
                case "0":
                    return;
                default:
                    Console.WriteLine("Please ensure you are entering a whole number from 1-10, or 0 to return to main menu");
                    break;
            }
        }
    }
    /////////////////////Private methods to do stuff like check if a variable is a BV, and find matrix inverse//////////////////////////
    
    private int IsBV(Table table, int ColIndex) //Returns the row where the 1 is for BV variables, otherwise returns -1;
    {
        int rows = table.table.GetLength(0);//Find the number of rows to iterate through
        int oneCount = 0;
        int oneRow = -1;//Use -1 as a placeholder for being a n NBV

        for (int i = 0; i < rows; i++)
        {
            if (table.table[i, ColIndex] == 1)//Check whether the current value is a 1
            {
                oneCount++;
                oneRow = i;
            }
            else if (table.table[i, ColIndex] != 0)//If any value is not a zero or a one this is not a BV
            {
                return -1;
            }
        }
        if (oneCount == 1)//If it is a BV, there will only be a single 1
        {
            return oneRow;
        }
        return -1; //Otherwise return -1;

    }

    private double[,] InverseMatrix(double[,] matrix)
    {
        int n = matrix.GetLength(0);

        // Start with augmented [matrix | I]
        double[,] augmented = new double[n, 2 * n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                augmented[i, j] = matrix[i, j];

            for (int j = n; j < 2 * n; j++)
                augmented[i, j] = (i == (j - n)) ? 1.0 : 0.0;
        }

        // Gauss–Jordan elimination
        for (int i = 0; i < n; i++)
        {
            // Find pivot
            double pivot = augmented[i, i];
            if (Math.Abs(pivot) < 1e-10)
            {
                // Singular → no inverse
                return null;
            }

            // Normalize pivot row
            for (int j = 0; j < 2 * n; j++)
                augmented[i, j] /= pivot;

            // Eliminate other rows
            for (int k = 0; k < n; k++)
            {
                if (k == i) continue;
                double factor = augmented[k, i];
                for (int j = 0; j < 2 * n; j++)
                    augmented[k, j] -= factor * augmented[i, j];
            }
        }

        // Extract inverse
        double[,] inverse = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                inverse[i, j] = augmented[i, j + n];
        }

        return inverse;
    }


    private double[,] Transpose(double[,] M)//Find the transpose of an array M - main T-transpose
    {
        int r = M.GetLength(0);
        int c = M.GetLength(1);
        double[,] T = new double[c, r];
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                T[j, i] = M[i, j];
        return T;
    }

    private double[] MultiplyMatrixVector(double[,] matrix, double[] vector)//Multiply 2 vectors with one another
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        if (vector.Length != cols)
            throw new ArgumentException("Matrix/vector size mismatch.");

        double[] result = new double[rows];
        for (int i = 0; i < rows; i++)
        {
            double sum = 0;
            for (int j = 0; j < cols; j++)
                sum += matrix[i, j] * vector[j];
            result[i] = sum;
        }
        return result;
    }

    private double OriginalObjectiveCoefficients(int variableCol)
    {
        Table firstTable = tables[0];
        double[] coeff = new double[firstTable.table.GetLength(1)];
        for(int i = 0; i < firstTable.table.GetLength(1); i++)
        {
            coeff[i] = firstTable.table[0,i];
        }
        
        return coeff[variableCol];
    }

    private List<int> BasicVariableIndices(Table table)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < table.table.GetLength(1); i++)
        {
            int oneCount = 0;
            for (int j = 0; j < table.table.GetLength(0); j++)
            {
                if (table.table[j, i] == 1)//Check whether the current value is a 1
                {
                    oneCount++;
                }
                else if (table.table[j, i] != 0)//If any value is not a zero or a one this is not a BV
                {
                    oneCount = 10;//This is very sketchy but it works!
                    break;
                }
            }
            if (oneCount == 1)//If it is a BV, there will only be a single 1
            {
                indices.Add(i);
            }
        }
        return indices;
    }

    // Solves A x = b using Gaussian elimination (no inversion) - This is also 100% ChatGPT
    private double[] SolveLinearSystem(double[,] A, double[] b)
    {
        int n = b.Length;
        double[,] mat = new double[n, n + 1];

        // Build augmented matrix [A | b]
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                mat[i, j] = A[i, j];
            mat[i, n] = b[i];
        }

        // Forward elimination
        for (int i = 0; i < n; i++)
        {
            // Pivot
            double pivot = mat[i, i];
            if (Math.Abs(pivot) < 1e-10)
                throw new InvalidOperationException("Matrix is singular or nearly singular.");

            // Normalize row
            for (int j = i; j <= n; j++)
                mat[i, j] /= pivot;

            // Eliminate below
            for (int k = i + 1; k < n; k++)
            {
                double factor = mat[k, i];
                for (int j = i; j <= n; j++)
                    mat[k, j] -= factor * mat[i, j];
            }
        }

        // Back substitution
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = mat[i, n];
            for (int j = i + 1; j < n; j++)
                x[i] -= mat[i, j] * x[j];
        }

        return x;
    }


    /////////////////////Private methods to find the ranges, shadow prices etc, using the methods above//////////////////////////
    private void DisplayNonBasicVariableRanges(Table finalTable, int ColIndex) //Check the row 0 to find how much they can change before becomming a pivot option
    {
        if (IsBV(finalTable, ColIndex) == -1) //IsBV method will return -1 if its a NBV. For this method, we do not care where the 1 is if it is a BV.
        {
            double reducedCost = finalTable.table[0, ColIndex];//Check the z row value of the selected index after confirming it is an NBV

            if (reducedCost > 0)
            {
                Console.WriteLine($"Range for index {ColIndex}: decrease up to {reducedCost}, increase = ∞");//If it is a positive value, how much can it be decreased before it becomes a pivot option.
            }
            else if (reducedCost < 0)
            {
                Console.WriteLine($"Range for index {ColIndex}: increase up to {Math.Abs(reducedCost)}, decrease = ∞");//Find the absolute value of which you can increase the number with before it becomes a pivot option.
            }
            else 
            {
                Console.WriteLine("This value has no range, but pivoting on it might give you another feasible solution!");
            } 
        }
        else
        {
            Console.WriteLine($"Cannot display NBV value range since the chosen index: "+ColIndex+" is not an NBV");
        }
    }

    private void DisplayBasicVariableRange(Table finalTable, int selectedCol)//Check the RHS value to find how much it can change before the current solution becomes infeasible
    {
        int oneRow = IsBV(finalTable, selectedCol);//For this method we must catch what row the 1 is in if it is a BV
        if (oneRow != -1)//If oneRow = -1, the selected index is an NBV
        {
            double currentValue = finalTable.table[oneRow, finalTable.table.GetLength(1) - 1];//CurrentValue is the optimal value for whatever index is selected

            double allowIncrease = double.PositiveInfinity;//If this is not assigned to a new value, you can change the value infinitely without having an issue
            double allowDecrease = double.PositiveInfinity;

            for (int i = 1; i < finalTable.table.GetLength(0); i++) // skip objective row
            {
                double aij = finalTable.table[i, selectedCol];
                double bi = finalTable.table[i, finalTable.table.GetLength(1) - 1];

                if (aij > 0)
                    allowIncrease = Math.Min(allowIncrease, bi / aij);
                else if (aij < 0)
                    allowDecrease = Math.Min(allowDecrease, -bi / aij);
            }

            Console.WriteLine($"x{selectedCol} range: [{currentValue - allowDecrease}, {currentValue + allowIncrease}]");
        }
        else
        {
            Console.WriteLine("Cannot display BV value range since the chosen index: " + selectedCol + " is not a BV");
        }
    }

    private void DisplayRHSRange(Table optimalTable, Table initialTable, int constraintRow) 
    {
        //Start by finding the number of basic variables
        int nBasic = 0;
        for (int i = 0; i < optimalTable.table.GetLength(1); i++)
        {
            if (IsBV(optimalTable, i) != -1)//If it is returned as a basic variable, increment the no of basic variables we have
            { 
                nBasic++;
            }
        }

        //Extract the entire RHS, since all RHS values have reference to a BV
        double[] RHS = new double[nBasic];
        for (int i = 0; i < nBasic; i++)
        {
            RHS[i] = optimalTable.table[i, optimalTable.table.GetLength(1) - 1];//Looping through the RHS value of each row and adding it to the array
        }

        //Constructing the B matrix
        double[,] B = new double[nBasic, nBasic];
        int colCounter = 0;
        for (int i = 0; i < optimalTable.table.GetLength(1); i++)
        {
            if (IsBV(optimalTable, i) != -1)//This is a basic variable we have found, we should add its column values to the B matrix
            {
                int rowCounter = 0;
                foreach (var value in initialTable.returnColumn(i))
                { 
                    B[rowCounter, colCounter] = value;
                    rowCounter++;
                }
            }
            colCounter++;
        }

        //Create the B inverse matrix (B^-1)
        double[,] B_inv = InverseMatrix(B);

        //Unit vector for the selected constraint
        double[] e = new double[nBasic];
        e[constraintRow] = 1;

        // Compute how xB changes with delta b
        double[] deltaX = MultiplyMatrixVector(B_inv, e); // implement or use a library

        // Compute allowable increase/decrease
        double maxIncrease = double.PositiveInfinity;
        double maxDecrease = double.PositiveInfinity;

        for (int i = 0; i < deltaX.Length; i++)
            {
                if (deltaX[i] > 0)
                    maxIncrease = Math.Min(maxIncrease, RHS[i] / deltaX[i]);
                else if (deltaX[i] < 0)
                    maxDecrease = Math.Min(maxDecrease, -RHS[i] / deltaX[i]);
            }

        double lowerBound = RHS[constraintRow] - maxDecrease;
        double upperBound = RHS[constraintRow] + maxIncrease;

        Console.WriteLine($"The lowerbound for the RHS of the selected row is: {lowerBound}");
        Console.WriteLine($"The upperbound for the RHS of the selected row is: {upperBound}");
    }

    public (double lowerBound, double upperBound) GetNonBasicVariableRange(Table optimalTable, int variableCol, bool isMaximization)
    {
        int numCols = optimalTable.table.GetLength(1);
        int numRows = optimalTable.table.GetLength(0);

        // Objective row (usually row 0 in the simplex tableau)
        double[] objRow = new double[numCols];
        for (int j = 0; j < numCols; j++)
            objRow[j] = optimalTable.table[0, j];

        double reducedCost = objRow[variableCol];

        double lowerBound, upperBound;

        if (isMaximization)
        {
            if (reducedCost > 0)
            {
                // It can decrease until zero
                lowerBound = -double.PositiveInfinity;
                upperBound = OriginalObjectiveCoefficients(variableCol);
            }
            else if (reducedCost < 0)
            {
                // It can increase until zero
                lowerBound = OriginalObjectiveCoefficients(variableCol);
                upperBound = double.PositiveInfinity;
            }
            else
            {
                // Already zero, it could enter immediately
                lowerBound = double.NegativeInfinity;
                upperBound = double.PositiveInfinity;
            }
        }
        else // Minimization
        {
            if (reducedCost < 0)
            {
                // It can increase until zero
                lowerBound = OriginalObjectiveCoefficients(variableCol);
                upperBound = double.PositiveInfinity;
            }
            else if (reducedCost > 0)
            {
                // It can decrease until zero
                lowerBound = double.PositiveInfinity;
                upperBound = OriginalObjectiveCoefficients(variableCol);
            }
            else
            {
                lowerBound = double.NegativeInfinity;
                upperBound = double.PositiveInfinity;
            }
        }

        return (lowerBound, upperBound);
    }

    public string AddNewActivity(Table optimalTable, double[] newColumn, double newObjectiveCoeff, bool isMaximization)
    {
        int numCols = optimalTable.table.GetLength(1);
        int numRows = optimalTable.table.GetLength(0);

        // Step 1: Build B from basic variables
        int nBasic = BasicVariableIndices(optimalTable).Count;


        double[,] B = new double[nBasic, nBasic];
        for (int i = 0; i < nBasic; i++)
        {
            int colIndex = BasicVariableIndices(optimalTable)[i];
            for (int j = 0; j < nBasic; j++)
                B[j, i] = optimalTable.table[j + 1, colIndex]; // +1 to skip objective row
        }

        // Step 2: Invert B
        double[,] B_inv = InverseMatrix(B);

        // Step 3: Get c_B (objective coeffs of basic variables)
        double[] cB = new double[nBasic];
        for (int i = 0; i < nBasic; i++)
            cB[i] = OriginalObjectiveCoefficients(i);

        // Step 4: Compute shadow prices y^T = cB^T * B^-1
        double[] y = MultiplyMatrixVector(Transpose(B_inv), cB);

        // Step 5: Compute reduced cost: c_new - y^T * a_new
        double dot = 0;
        for (int i = 0; i < newColumn.Length; i++)
            dot += y[i] * newColumn[i];

        double reducedCost = newObjectiveCoeff - dot;

        // Step 6: Check optimality
        if (isMaximization)
        {
            if (reducedCost >= 0)
                return "New activity does not improve the solution. Optimal solution unchanged.";
            else
                return $"New activity can improve the solution. Reduced cost = {reducedCost}. Run one simplex pivot.";
        }
        else // Minimization
        {
            if (reducedCost <= 0)
                return "New activity does not improve the solution. Optimal solution unchanged.";
            else
                return $"New activity can improve the solution. Reduced cost = {reducedCost}. Run one simplex pivot.";
        }
    }

    public string AddNewConstraint(Table optimalTable, double[] newConstraint, double rhs)
    {
        int nVars = optimalTable.table.GetLength(1) - 1; // exclude RHS column

        // Extract current solution (decision variable values)
        double[] xStar = new double[nVars];
        for (int j = 0; j < nVars; j++)
        {
            if (BasicVariableIndices(optimalTable).Contains(j))
            {
                int rowIndex = BasicVariableIndices(optimalTable).IndexOf(j) + 1;
                xStar[j] = optimalTable.table[rowIndex, optimalTable.table.GetLength(1) - 1]; // RHS
            }
            else
            {
                xStar[j] = 0.0;
            }
        }

        // Compute LHS of new constraint
        double lhs = 0;
        for (int j = 0; j < nVars; j++)
            lhs += newConstraint[j] * xStar[j];

        // Step 2: Check feasibility
        if (lhs <= rhs)
        {
            return "New constraint is satisfied by the current optimal solution. Optimal solution unchanged.";
        }
        else
        {
            return "New constraint cuts off the current solution. Need to re-optimize using Dual Simplex.";
        }
    }
    // Computes shadow prices (dual values) without explicit matrix inversion
    public double[] ComputeShadowPrices(Table optimalTable)
    {
        int m = optimalTable.table.GetLength(0) - 1; // #constraints (excluding objective row)

        // --- Build B from tableau ---
        int nBasic = BasicVariableIndices(optimalTable).Count;
        if (nBasic != m)
            throw new InvalidOperationException("Basis size must equal number of constraints.");

        double[,] B = new double[m, m];
        for (int i = 0; i < nBasic; i++)
        {
            int colIndex = BasicVariableIndices(optimalTable)[i];
            for (int r = 0; r < m; r++)
            {
                B[r, i] = optimalTable.table[r + 1, colIndex]; // skip objective row
            }
        }

        // --- c_B (objective coeffs of basic variables) ---
        double[] cB = new double[m];
        for (int i = 0; i < m; i++)
        {
            int varCol = BasicVariableIndices(optimalTable)[i];
            cB[i] = OriginalObjectiveCoefficients(varCol);
        }

        // --- Solve B^T y = cB instead of inverting B ---
        double[] y = SolveLinearSystem(Transpose(B), cB);

        return y;
    }

};
