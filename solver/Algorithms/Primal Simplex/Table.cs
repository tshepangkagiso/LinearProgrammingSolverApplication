namespace solver;

public class Table
{
    public double[,] table;

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
}
