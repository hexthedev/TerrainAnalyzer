using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeDegreeComparer : IComparer<VoronoiNode>
{
    public int Compare(VoronoiNode x, VoronoiNode y)
    {
        int xCount = x.getNeighbours().Count;
        int yCount = y.getNeighbours().Count;

        if (xCount > yCount)
        {
            return -1;
        } else if (xCount < yCount)
        {
            return 1;
        } else
        {
            return 0;
        }
    }
}
