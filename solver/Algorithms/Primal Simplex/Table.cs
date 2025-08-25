namespace solver;

internal class Table
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
}
