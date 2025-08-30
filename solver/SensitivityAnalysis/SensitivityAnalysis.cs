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
<<<<<<< Updated upstream
                Console.WriteLine("TODO: Show range of non-basic variable");
=======
            {
                Console.WriteLine("====================================================================");
                Console.WriteLine("Please enter the index of the value you want to find the range of (0-?)");
                int index = int.Parse(Console.ReadLine()); //Do some error handling later on
                DisplayNonBasicVariableRanges(tables[tables.Count - 1], index);//Passing it the optimal table, and the ColIndex of the range to be found
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
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

    private double[,] InverseMatrix(double[,] matrix)//This method is 100% chatGPT
    {
        int n = matrix.GetLength(0);

        // Start with the augmented matrix [matrix | I]
        double[,] augmented = new double[n, 2 * n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                augmented[i, j] = matrix[i, j];   // copy original matrix

            for (int j = n; j < 2 * n; j++)
                augmented[i, j] = (i == (j - n)) ? 1.0 : 0.0; // identity matrix
        }

        // Perform Gauss–Jordan elimination
        for (int i = 0; i < n; i++)
        {
            // Find pivot
            double pivot = augmented[i, i];
            if (Math.Abs(pivot) < 1e-10)
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");

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

        // Extract inverse matrix (right half of augmented matrix)
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
        int r = M.GetLength(0), c = M.GetLength(1);
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

        //Continue here
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
        int nBasic = optimalTable.BasicVariableIndices.Count;


        double[,] B = new double[nBasic, nBasic];
        for (int i = 0; i < nBasic; i++)
        {
            int colIndex = optimalTable.BasicVariableIndices[i];
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
            if (optimalTable.BasicVariableIndices.Contains(j))
            {
                int rowIndex = optimalTable.BasicVariableIndices.IndexOf(j) + 1;
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

    // Computes y^T = c_B^T * B^{-1} and returns y (length = #constraints)
    public double[] ComputeShadowPrices(Table optimalTable)
    {
        // m = number of constraints (rows excluding the objective row)
        int m = optimalTable.table.GetLength(0) - 1;

        // --- Build B from the optimal tableau (skip objective row at index 0) ---
        int nBasic = optimalTable.BasicVariableIndices.Count;
        if (nBasic != m)
            throw new InvalidOperationException("Basis size must equal number of constraints.");

        double[,] B = new double[m, m];
        for (int i = 0; i < nBasic; i++)
        {
            int colIndex = optimalTable.BasicVariableIndices[i];
            for (int r = 0; r < m; r++)
            {
                // Row r in constraints = tableau row (r+1) because row 0 is the objective
                B[r, i] = optimalTable.table[r + 1, colIndex];
            }
        }

        // --- c_B: the objective coeffs of the basic variables ---
        double[] cB = new double[m];
        for (int i = 0; i < m; i++)
        {
            int varCol = optimalTable.BasicVariableIndices[i];
            cB[i] = OriginalObjectiveCoefficients(varCol);
        }

        // --- y^T = c_B^T * B^{-1}  →  y = (B^{-T}) * c_B ---
        // We'll compute y = Transpose(B_inv) * cB to reuse the MultiplyMatrixVector helper.
        double[,] B_inv = InverseMatrix(B);
        double[,] B_inv_T = Transpose(B_inv);
        double[] y = MultiplyMatrixVector(B_inv_T, cB);

        return y; // shadow price for each constraint, in tableau row order (1..m)
    }

>>>>>>> Stashed changes
}
