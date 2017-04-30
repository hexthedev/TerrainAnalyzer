using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeRadiusComparer : IComparer<VoronoiNode>
{
    public int Compare(VoronoiNode x, VoronoiNode y)
    {
        if(x.getRadius() > y.getRadius())
        {
            return -1;
        } else if (x.getRadius() < y.getRadius())
        {
            return 1;
        } else
        {
            return 0;
        }
    }
}
