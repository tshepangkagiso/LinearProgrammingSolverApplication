namespace solver;

internal class Minimize : Pivot
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

    public Minimize(Table table) { }
    public Minimize() { }
    public int findPivotRow(Table obj)//Returns -1 in case of unbounded solution
    {
        int pivotRow = -1;
        double highestNegetive = 0;
        for (int i = 1; i < obj.table.GetLength(0); i++)//Loop through all rows except the first one
        {

            double currentRow = obj.table[i, obj.table.GetLength(1) - 1];//gets the current RHS value
            if (currentRow < 0)//Only check negetive values
            {
                if (currentRow < highestNegetive)//Check if the current RHS is less than the highest negetive found so far
                {
                    //output += $"New highest negetive found: {currentRow} on row {i}\n";
                    highestNegetive = currentRow;
                    pivotRow = i;
                }
            }
        }
        //output += $"Pivot row is {pivotRow}\n";
        return pivotRow;
    }

    public int findPivotColumn(Table obj, int pivotRow)//Returns -1 in case of being optimal
    {
        pivotColumn = -1;
        //output += $"Finding pivot column for row {pivotRow}\n";
        double lowestRatio = double.MaxValue;
        for (int i = 0; i < obj.table.GetLength(1) - 1; i++)//Loop through the first row in the array. -1 since we do not look at RHS values in primal simplex
        {
            double ratio = obj.table[0, i] / obj.table[pivotRow, i];
            //output += $"Ratio for column {i}: {ratio}\n";
            if ((ratio < lowestRatio))//Check what index has the lowest positive ratio
            {
                //output += $"Ratio for column {i}: {ratio}\n new lowest ratio is on row {pivotRow} and column {i}";
                lowestRatio = obj.table[pivotRow, i];
                pivotColumn = i;
            }
        }
        return pivotColumn;
    }



    public override Table findNextTable(Table currentTable, int pivotRow, int pivotColumn)
    {
        Table newTable = new Table(new double[currentTable.table.GetLength(0), currentTable.table.GetLength(1)]);
        double pivotValue = currentTable.getVariableValue(pivotRow, pivotColumn);
        for (int i = 0; i < currentTable.table.GetLength(0); i++)//Loop through all rows
        {
            for (int j = 0; j < currentTable.table.GetLength(1); j++)//Loop through all columns
            {
                if (i == pivotRow && j == pivotColumn)//If we are at the pivot element
                {
                    newTable.setVariableValue(i, j, 1);//Set the pivot element to 1
                }
                else if (i == pivotRow)//If we are in the pivot row
                {
                    newTable.setVariableValue(i, j, currentTable.getVariableValue(i, j) / pivotValue);//Divide the entire row by the pivot value
                }
                else if (j == pivotColumn)//If we are in the pivot column
                {
                    newTable.setVariableValue(i, j, 0);//Set the entire column to 0
                }
                else//If we are not in the pivot row or column
                {
                    double newValue = currentTable.getVariableValue(i, j) - (currentTable.getVariableValue(pivotRow, j) * (currentTable.getVariableValue(i, pivotColumn) / pivotValue));//Calculate the new value using the formula
                    newTable.setVariableValue(i, j, newValue);//Set the new value in the new table
                }
            }
        }
        return newTable;
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

    public void minimize()
    {
        tables.Add(table);
        int pivotRow = findPivotRow(table);
        while (pivotRow != -1)//While the table is not optimal
        {
            int pivotColumn = findPivotColumn(table, pivotRow);
            if (pivotColumn == -1)//If there is no valid pivot column, the solution is unbounded
            {
                break;
            }
            table = findNextTable(table, pivotRow, pivotColumn);
            tables.Add(table);
            pivotRow = findPivotRow(table);
        }
        printTables();
        for (int i = 1; i < table.table.GetLength(0); i++)
        {
            Console.WriteLine("Variable " + i + ": " + table.getVariableValue(i, table.table.GetLength(1) - 1) + "\n");
        }
    }
}

