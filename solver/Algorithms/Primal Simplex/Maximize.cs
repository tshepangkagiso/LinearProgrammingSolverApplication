namespace solver;

internal class Maximize : Pivot
{
    private int pivotColumn;
    private List<Table> tables = new List<Table>();//This list will contain table objects from T-i to T*
    private Table table = new Table(new double[,]
    {
        { -3, -2, 0, 0, 0 },
        {  2,  1, 1, 0, 0 },
        {  1,  1, 0, 1, 0 },
        {  1,  0, 0, 0, 1 }
    }); //Create a testTable array using Santa's values for testing purposes
    public Maximize()
    {
        maximize();
    }

    public override int findPivotColumn(Table table)//Returns -1 in case of being optimal
    {
        pivotColumn = -1;
        double highestNegetive = 0;
        for (int i = 0; i < table.table.GetLength(1) - 1; i++)//Loop through the first row in the array. -1 since we do not look at RHS values in primal simplex
        {
            if ((table.table[0, i] < highestNegetive))//Check what index has the highest negetive value
            {
                highestNegetive = table.table[0, i];
                pivotColumn = i;
            }
        }
        return pivotColumn;
    }

    public override int findPivotRow(Table table, int pivotColumn)//Returns -1 in case of unbounded solution
    {
        int pivotRow = -1;
        double lowestRatio = double.MaxValue;
        for (int i = 1; i < table.table.GetLength(0); i++)//Loop through all rows except the first one
        {
            if (table.table[i, pivotColumn] > 0)//Only consider positive values in the pivot column
            {
                double ratio = table.table[i, table.table.GetLength(1) - 1] / table.table[i, pivotColumn];//Calculate the ratio of RHS to the pivot column value
                if (ratio < lowestRatio)//Check if the ratio is the lowest one found so far
                {
                    lowestRatio = ratio;
                    pivotRow = i;
                }
            }
        }
        return pivotRow;
    }

    public override Table findNextTable(Table currentTable, int pivotRow, int pivotColumn)
    {//Finding next table using pivotRow, and pivotColumn 
        Console.WriteLine($"Startinng next pivot on: {currentTable.getVariableValue(pivotRow, pivotColumn):F2} at Row: {pivotRow}, Column: {pivotColumn}");
        double pivotValue = currentTable.getVariableValue(pivotRow, pivotColumn);
        double[,] nextTableArray = new double[currentTable.table.GetLength(0), currentTable.table.GetLength(1)];
        for (int i = 0; i < currentTable.table.GetLength(0); i++)//Loop through all rows
        {
            for (int j = 0; j < currentTable.table.GetLength(1); j++)//Loop through all columns
            {
                if (i == pivotRow && j == pivotColumn)//If we are at the pivot point
                {
                    nextTableArray[i, j] = 1;//Set the value to 1
                }
                else if (i == pivotRow)//If we are in the pivot row
                {
                    nextTableArray[i, j] = currentTable.getVariableValue(i, j) / pivotValue;//Divide the entire row by the pivot value
                }
                else if (j == pivotColumn)//If we are in the pivot column
                {
                    nextTableArray[i, j] = 0;//Set the entire column to 0
                }
                else//For all other values
                {
                    nextTableArray[i, j] = currentTable.getVariableValue(i, j) - ((currentTable.getVariableValue(i, pivotColumn) * currentTable.getVariableValue(pivotRow, j)) / pivotValue);//Apply the formula to get the new value
                }
            }
        }
        Table nextTable = new Table(nextTableArray);
        tables.Add(nextTable);
        return nextTable;
    }

    public String printTables()// prints out all tables in the tables list
    {
        String output = "";
        for (int t = 0; t < tables.Count; t++)
        {
            Console.WriteLine($"Table {t + 1}:");
            output += $"Table {t + 1}:\n";

            for (int i = 0; i < tables[t].table.GetLength(0); i++)
            {
                for (int j = 0; j < tables[t].table.GetLength(1); j++)
                {
                    Console.Write($"{tables[t].getVariableValue(i, j),8:F2} ");
                    output += $"{tables[t].getVariableValue(i, j),8:F2} ";
                }
                Console.WriteLine();
                output += "\n";
            }
            Console.WriteLine();
            output += "\n";

        }
        return output;
    }
    public void maximize()
    {
        tables.Add(table);
        int pivotRow, pivotColumn;
        do
        {
            pivotColumn = findPivotColumn(tables.Last());
            if (pivotColumn == -1) break;//If optimal, break the loop
            pivotRow = findPivotRow(tables.Last(), pivotColumn);
            if (pivotRow == -1)
            {
                Console.WriteLine("The solution is unbounded.");
                return;//If unbounded, exit the method
            }
            findNextTable(tables.Last(), pivotRow, pivotColumn);
        } while (true);
        printTables();
        Console.WriteLine($"Optimal value found: {tables.Last().table[0, tables.Last().table.GetLength(1) - 1]:F2}");
    }

}

