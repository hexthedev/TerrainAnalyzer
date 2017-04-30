using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCoordinate {

    public int x;
    public int y;

    public GridCoordinate(int pX, int pY)
    {
        x = pX;
        y = pY;
    }

    public static bool equals(GridCoordinate a, GridCoordinate b)
    {
        if(a.x == b.x && a.y == b.y)
        {
            return true;
        }

        return false;
    }


}
