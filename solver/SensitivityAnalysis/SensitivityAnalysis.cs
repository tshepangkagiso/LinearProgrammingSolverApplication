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

        string goal = "max";
        List<Table> tables = new List<Table>();

        Table table = new Table(new double[,]
        {
            { -3, -2, 0, 0, 0, 0 },
            {  2,  1, 1, 0, 0, 100 },
            {  1,  1, 0, 1, 0, 80 },
            {  1,  0, 0, 0, 1, 40 }
        });

        Table table2 = new Table(new double[,]
{
            { 0, -2, 0, 0, 3, 120 },
            {  0,  1, 1, 0, -2, 20 },
            {  0,  1, 0, 1, -1, 40 },
            {  1,  0, 0, 0, 1, 40 }
        });

        Table table3 = new Table(new double[,]
        {
            { 0, 0, 2, 0, -1, 160 },
            {  0,  1, 1, 0, -2, 20 },
            {  0,  0, -1, 1, 1, 20 },
            {  1,  0, 0, 0, 1, 40 }
        });

        Table table4 = new Table(new double[,]
        {
            { 0, 0, 1, 1, 0, 180 },
            {  0,  1, -1, 2,0, 60 },
            {  0,  0, -1, 1, 1, 20 },
            {  1,  0, 1, -1, 0, 20 }
        });

        tables.Add(table);
        tables.Add(table2);
        tables.Add(table3);
        tables.Add(table4);

        string choice = Console.ReadLine();
        /*
        switch (choice)
        {
   /        case "1":
            {
                Console.WriteLine("====================================================================");
                Console.WriteLine("Please enter the index of the value you want to find the range of (0-?)");
                int index = int.Parse(Console.ReadLine()); //Do some error handling later on
                //DisplayNonBasicVariableRanges(tables[tables.Count] - 1, index);//Passing it the optimal table, and the ColIndex of the range to be found
                break;
            }
            case "2":
            {
                //ToDo: Requires posibility of more pivots
                break;
            }
            case "3":
            {
                
                break;
            }

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

    private void DisplayNonBasicVariableRanges(Table finalTable, int ColIndex) //Check the row 0 to find how much they can change before becomming a pivot option
    {
        if (IsBV(finalTable, ColIndex) == -1) //IsBV method will return -1 if its a NBV. For this method, we do not care where the 1 is if it is a BV.
        {
            double reducedCost = finalTable[0, ColIndex];//Check the z row value of the selected index after confirming it is an NBV

            if (reducedCost > 0)
            {
                Console.WriteLine($"Range for index {ColIndex}: decrease up to {reducedCost}, increase = ∞")//If it is a positive value, how much can it be decreased before it becomes a pivot option.
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
            Console.WriteLine('Cannot display NBV value range since the chosen index: '+ColIndex+' is not an NBV');
        }
    }

    private void DisplayBasicVariableRange(Table finalTable, int selectedCol)//Check the RHS value to find how much it can change before the current solution becomes infeasible
    {
        int oneRow = IsBV(finalTable, selectedCol);//For this method we must catch what row the 1 is in if it is a BV
        if (oneRow != -1)//If oneRow = -1, the selected index is an NBV
        {
            double currentValue = finalTable.table[oneRow, finalTable.table.GetLength(1) - 1];//CurrentValue is the optimal value for whatever index is selected

            double allowIncrease = double.PositiveInfinity;
            double allowDecrease = double.PositiveInfinity;

            for (int i = 1; i < table.GetLength(0); i++) // skip objective row
            {
                double aij = table[i, selectedCol];
                double bi = table[i, table.GetLength(1) - 1];

                if (aij > 0)
                    allowIncrease = Math.Min(allowIncrease, bi / aij);
                else if (aij < 0)
                    allowDecrease = Math.Min(allowDecrease, -bi / aij);
            }

            Console.WriteLine($"x{selectedCol} range: [{currentValue - allowDecrease}, {currentValue + allowIncrease}]");
        }
        else
        {
            Console.WriteLine('Cannot display BV value range since the chosen index: ' + ColIndex + ' is not a BV');
        }
    }

    private void DisplayRHSRange(Table optimalTable, int constraintRow) 
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
        for (int i = 0; i < nBasic; i++)
        { 
            //Continue here
        }*/
    }
}
