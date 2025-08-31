namespace solver;

internal class Pivot
{
    private Table table = new Table(new double[,]
{
        { -3, -2, 0, 0, 0 },
        {  2,  1, 1, 0, 0 },
        {  1,  1, 0, 1, 0 },
        {  1,  0, 0, 0, 1 }
    }); //Create a testTable array using Santa's values for testing purposes

    private int pivotColumn;
    private int pivotRow;

    public virtual int findPivotColumn(Table obj)//This message will get overriden with maximize/minimize class. 
    {
        return -1;//If this value is not overriden, there is no fitting value in z row. (optimal/infeasible)
    }

    public virtual int findPivotRow(Table obj, int pivotColumn)//Returns -1 in case of being unbounded
    {
        pivotRow = -1;
        double lowestpositive = -1;
        this.pivotColumn = pivotColumn;
        for (int i = 0; i < obj.table.GetLength(0); i++)//Loop through all rows
        {
            double ratioValue = obj.getVariableValue(i, obj.table.GetLength(0) - 1);//Get the value of the RHS of the current row
            if (ratioValue < lowestpositive)
            {
                if (ratioValue == 0)//check for degenerecy
                {
                    if (obj.getVariableValue(i, pivotColumn) > 0)//Check that value in the pivot column is positive
                    {
                        lowestpositive = ratioValue;
                        pivotRow = i;
                    }
                }
                else if (ratioValue > 0)//make sure its a positive value
                {
                    lowestpositive = ratioValue;
                    pivotRow = i;
                }
            }
        }
        return pivotRow;
    }

    public virtual Table findNextTable(Table currentTable, int pivotRow, int pivotColumn) {//Finding next table using pivotRow, and pivotColumn 
        return null;
    }

}
