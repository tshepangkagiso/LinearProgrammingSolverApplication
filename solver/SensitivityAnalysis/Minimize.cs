namespace solver;

internal class Minimize:Pivot
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

    public Minimize() { }

    public override int findPivotColumn(Table obj)//Returns -1 in case of being optimal
    {
        pivotColumn = -1;
        double lowestNegative = 0;
        for (int i = 0; i < obj.table.GetLength(1) - 1; i++)//Loop through the first row in the array. -1 since we do not look at RHS values in primal simplex
        {
            if ((obj.table[0, i] < lowestNegative))//Check what index has the lowest negative value
            {
                lowestNegative = obj.table[0, i];
                pivotColumn = i;
            }
        }
        return pivotColumn;
    }
}
