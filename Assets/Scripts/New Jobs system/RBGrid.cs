using System;
using System.Collections.Generic;
using Unity.Mathematics;


[Serializable]
public class Grid<T> where T : GridElement
{
    public int RowsCount, ColumnsCount;

    public Row<T>[] Rows;
    private List<T> _adjustantTiles = new List<T>();
    public Grid(int rows, int column)
    {
        RowsCount = rows;
        ColumnsCount = column;

        Rows = new Row<T>[rows];

        for (int i = 0; i < rows; i++)
        {
            Rows[i] = new Row<T>(column);
        }
    }
    public void Add(int row, int column, T _object)
    {
        Rows[row].Column[column] = _object;
    }
    public List<T> GetAdjustantTiles(int2 index)
    {
        //if (element == null) return null;
        //int2 index = element.Index;
        _adjustantTiles.Clear();
        List<T> at = new List<T>();
        if (CheckCellIndex(index.x, index.y - 1)) at.Add(GetCell(index.x, index.y - 1));//Top

        if (CheckCellIndex(index.x, index.y + 1)) at.Add(GetCell(index.x, index.y + 1));//down

        if (CheckCellIndex(index.x + 1, index.y)) at.Add(GetCell(index.x + 1, index.y)); // right

        if (CheckCellIndex(index.x - 1, index.y)) at.Add(GetCell(index.x - 1, index.y)); // left

        if (CheckCellIndex(index.x - 1, index.y - 1)) at.Add(GetCell(index.x - 1, index.y - 1)); // topLeft

        if (CheckCellIndex(index.x + 1, index.y - 1)) at.Add(GetCell(index.x + 1, index.y - 1)); // topRight

        if (CheckCellIndex(index.x - 1, index.y + 1)) at.Add(GetCell(index.x - 1, index.y + 1)); // downLeft

        if (CheckCellIndex(index.x + 1, index.y + 1)) at.Add(GetCell(index.x + 1, index.y + 1)); // downRight

        //at.Top = GetCell(index.x, index.y - 1);
        //at.down = GetCell(index.x, index.y + 1);
        //var right = GetCell(index.x + 1, index.y);
        //var left = GetCell(index.x - 1, index.y);

        //var topLeft = GetCell(index.x - 1, index.y - 1);
        //var topRight = GetCell(index.x + 1, index.y - 1);

        //var downLeft = GetCell(index.x - 1, index.y + 1);
        //var downRight = GetCell(index.x + 1, index.y + 1);


        return at;
    }
    public T GetCell(int row, int column)
    {
        if (!CheckCellIndex(row, column)) return default;

        return Rows[row].Column[column];
    }
    private bool CheckCellIndex(int row, int column)
    {
        if (row < 0 || column < 0) return false;
        if (row >= RowsCount || column >= ColumnsCount) return false;
        return true;
    }

}
[Serializable]
public class Row<T>
{
    public T[] Column;

    public Row(int column)
    {
        Column = new T[column];
    }
}