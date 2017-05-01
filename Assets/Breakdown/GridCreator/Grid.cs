using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T> {

    private T[,] aGrid;

    private Vector3 aOrigin;
    private Vector3 aXStep;
    private Vector3 aYStep;

    public Grid(int width, int height, Vector3 origin, Vector3 xStep, Vector3 yStep)
    {
        aGrid = new T[width, height];
        aOrigin = origin;
        aXStep = xStep;
        aYStep = yStep;
    }

    public void fillGrid(GridSquareCalculator<T> calculator)
    {
        for (int i = 0; i < aGrid.GetLength(0); i++)
        {
            for(int j = 0; j< aGrid.GetLength(1); j++)
            {
                aGrid[i, j] = calculator.getValue(aOrigin + aXStep * i + aYStep * j);
            }
        }
    }

    public Vector3 gridToWorld(int x, int y)
    {
        return aOrigin + aXStep * x + aYStep * y;
    }

    public T[ , ] getGrid()
    {
        return aGrid;
    }

}
