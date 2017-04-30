using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiNode {

    private Vector3 position;
    private List<VoronoiNode> neighbours;
    private bool isLeaf;
    private float height;
    private float radius;

    public VoronoiNode(Vector3 pPosition)
    {
        position = pPosition;
        neighbours = new List<VoronoiNode>();
        isLeaf = true;
    }

    public void addNeighbour(VoronoiNode pNeighbour)
    {
        bool alreadyThere = false;

        foreach(VoronoiNode node in neighbours)
        {
            if(pNeighbour.getPosition() == position || pNeighbour.getPosition() == node.getPosition())
            {
                alreadyThere = true;
                break;
            }
        }
        
        if (!alreadyThere)
        {
            neighbours.Add(pNeighbour);
        }

        if (neighbours.Count > 1)
        {
            isLeaf = false;
        }

    }

    public void removeNeighbour(VoronoiNode pNeighbour)
    {
        neighbours.Remove(pNeighbour);
        
        if (neighbours.Count <= 1)
        {
            isLeaf = true;
        }
    }

    public void removeAllNeighbours()
    {
        neighbours = new List<VoronoiNode>();
        isLeaf = true;
    }

    public bool isLeafNode()
    {
        return isLeaf;
    }

    public void setRadius(float pRadius)
    {
        radius = pRadius;
    }

    public float getRadius()
    {
        return radius;
    }

    public void setHeight(float pHeight)
    {
        height = pHeight;
    }

    public float getHeight()
    {
        return height;
    }


    //For printing
    public List<VoronoiNode> getNeighbours()
    {
        return neighbours;
    }

    public Vector3 getPosition()
    {
        return position;
    }
}
