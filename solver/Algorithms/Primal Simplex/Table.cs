namespace solver;

public class Table
{
    public double[,] table;
    public List<int> BasicVariableIndices;

    public Table() { }

    public Table(double[,] table)
    {
        this.table = table;
    }

    public void setVariableValue(int rowIndex, int colIndex, double value)
    {
        this.table[rowIndex, colIndex] = value;
    }

    public double getVariableValue(int rowIndex, int colIndex)
    {
        return this.table[rowIndex, colIndex];
    }

    public void setTable(double[,] table)
    {
        this.table = table;
    }

    public double[,] returnColumn(int colIndex)
    {
        double[,] columnValues = new double[1,table.GetLength(0) - 1];

        for (int i = 1; i < table.GetLength(0); i++)//we start at 1 since we want to skip the z row
        {
            columnValues[0, i] = table[i, colIndex];
        }

        return columnValues;
    }

    public void setBasicVariableIndices()
    {
        int oneCount = 0;
        for(int i = 0; i < table.GetLength(1);  i++)
        {
            for (int j = 0; j < table.GetLength(0); j++)
            {
                if (table[j, i] == 1)//Check whether the current value is a 1
                {
                    oneCount++;
                }
                else if (table[j, i] != 0)//If any value is not a zero or a one this is not a BV
                {
                   break;
                }
            }
            if (oneCount == 1)//If it is a BV, there will only be a single 1
            {
                BasicVariableIndices.Add(i);
            }
        }
    }
}
