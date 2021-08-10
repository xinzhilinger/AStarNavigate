using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarLookRode
{
    public GridMeshCreate meshMap;
    public Grid startGrid;
    public Grid endGrid;

    public List<Grid> openGrids;
    public List<Grid> closeGrids;
    public Stack<Grid> rodes;

    public void Init(GridMeshCreate meshMap, Grid startGrid, Grid endGrid)
    {
        this.meshMap = meshMap;
        this.startGrid = startGrid;
        this.endGrid = endGrid;
        openGrids = new List<Grid>();
        closeGrids = new List<Grid>();
        rodes = new Stack<Grid>();
    }

    
    public IEnumerator OnStart()
    {

        //Item itemRoot = Map.bolls[0].item;
        rodes.Push(startGrid);
        closeGrids.Add(startGrid);

        TraverseItem(startGrid.posX, startGrid.posY);
        yield return new WaitForSeconds(1);
        Traverse();

        //为了避免无法完成寻路而跳不出循环的情况，使用For来规定寻路的最大步数
        for (int i = 0; i < 6000; i++)
        {
            if (rodes.Peek().posX == endGrid.posX && rodes.Peek().posY == endGrid.posY)
            {
                GetRode();
                break;
            }

            TraverseItem(rodes.Peek().posX, rodes.Peek().posY);
            yield return new WaitForSeconds(0.2f);
            Traverse();

        }

    }
    


    /// <summary>
    /// 寻找走到当前点位时，下一步所有可以走的相邻位置并加入到开放列表
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public void TraverseItem(int i, int j)
    {
        int xMin = Mathf.Max(i - 1, 0);
        int xMax = Mathf.Min(i + 1, meshMap.meshRange.horizontal - 1);
        int yMin = Mathf.Max(j - 1, 0);
        int yMax = Mathf.Min(j + 1, meshMap.meshRange.vertical - 1);

        Grid item = meshMap.grids[i, j].GetComponent<Grid>();
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                Grid grid = meshMap.grids[x, y].GetComponent<Grid>();
                if ((y == j && i == x) || closeGrids.Contains(grid))
                {
                    continue;
                }

                if (openGrids.Contains(grid))
                {
                    if (item.All > GetLength(grid, item))
                    {
                        item.parentGrid = grid;
                        SetNoteData(item);
                    }                    
                    continue;
                }
                    
                if (!grid.isHinder)
                {
                    openGrids.Add(grid);
                    grid.ChangeColor(Color.blue);
                    grid.parentGrid= item;
                }
               
            }
        }



    }

    /// <summary>
    /// 在开放列表选中路径最短的点加入的路径栈，同时将路径点加入到闭合列表中
    /// </summary>
    public void Traverse()
    {
        if (openGrids.Count == 0)
        {
            return;
        }
        Grid minLenthGrid = openGrids[0];
        int minLength = SetNoteData(minLenthGrid);
        for (int i = 0; i < openGrids.Count; i++)
        {
            if (minLength > SetNoteData(openGrids[i]))
            {
                minLenthGrid = openGrids[i];
                minLength = SetNoteData(openGrids[i]);
            }
        }
        minLenthGrid.ChangeColor(Color.green);
        Debug.Log("我在寻找人生的方向" + minLenthGrid.posX + "::::" + minLenthGrid.posY);

        closeGrids.Add(minLenthGrid);
        openGrids.Remove(minLenthGrid);               
        rodes.Push(minLenthGrid);
    }


    void GetRode()
    {
        List<Grid> grids = new List<Grid>();
        rodes.Peek().ChangeColor(Color.black);
        grids.Insert(0, rodes.Pop());
        while (rodes.Count != 0)
        {
            if (grids[0].parentGrid != rodes.Peek())
            {
                rodes.Pop();

            }
            else
            {
                rodes.Peek().ChangeColor(Color.black);
                grids.Insert(0, rodes.Pop());               
            }

        }      
    }
    public void AddGridToRode(Grid grid)
    {
        while (true)
        {
            if (rodes.Count==0 || rodes.Peek() == grid.parentGrid)
            {
                break;
            }
            rodes.Pop();
        }
        rodes.Push(grid);
    }


    /// <summary>
    /// 用来计算某一点位的预估路径总长度
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    public int SetNoteData(Grid grid)
    {

        Grid itemParent = rodes.Count == 0 ? startGrid : grid.parentGrid;
        int numG = Mathf.Abs(itemParent.posX - grid.posX) + Mathf.Abs(itemParent.posY - grid.posY);
        int n = numG == 1 ? 10 : 14;
        grid.G = itemParent.G + n;

        int numH = Mathf.Abs(endGrid.posX - grid.posX) + Mathf.Abs(endGrid.posY - grid.posY);
        grid.H = numH * 10;
        grid.All = grid.H + grid.G;
        return grid.All;
    }


    public int GetLength(Grid bejinGrid,Grid grid)
    {

        int numG = Mathf.Abs(bejinGrid.posX - grid.posX) + Mathf.Abs(bejinGrid.posY - grid.posY);
        int n = numG == 1 ? 10 : 14;
        int G = bejinGrid.G + n;

        int numH = Mathf.Abs(endGrid.posX - grid.posX) + Mathf.Abs(endGrid.posY - grid.posY);
        int H = numH * 10;
        int All = grid.H + grid.G;
        return All;
    }


}
