using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleGridCreator : MonoBehaviour {

    public int width;
    public int height;
    public Vector3 origin;
    public Vector3 xStep;
    public Vector3 yStep;

    public RayCastHeight calc;

    private Grid<float> grid;

    void Start()
    {
        grid = new Grid<float>(width, height, origin, xStep, yStep);
        grid.fillGrid(calc);
    }

    private void OnDrawGizmos()
    {
        float[,] testGrid = grid.getGrid();

        for(int i = 0; i < testGrid.GetLength(0); i++)
        {
            for(int j = 0; j< testGrid.GetLength(1); j++)
            {
                Gizmos.DrawSphere(grid.gridToWorld(i, j) + new Vector3(0,testGrid[i,j],0), 2f);
            }
        }

    }


}
