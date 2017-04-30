using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableNode {

    private Vector3 position;
    private GridCoordinate coordinate;
    private TraversableNode[] movableNodes = new TraversableNode[8];
    private bool traversable;
    
    public TraversableNode(Vector3 pPosition, GridCoordinate pCoordinate, bool isTraversable)
    {
        position = pPosition;
        coordinate = pCoordinate;
        traversable = isTraversable;
    }

    #region Basic Methods
    public TraversableNode getNeighbour(Direction direction)
    {
        return movableNodes[DirectionMethods.directionToArrayIndex(direction)];
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public GridCoordinate getCoordinate()
    {
        return coordinate;
    }
    
    public bool isTraversable()
    {
        return traversable;
    }
    
    public void setTraversable(bool setTo)
    {
        traversable = setTo;
    }
    #endregion

    //Insert a Traversable node in the direction slot this node can get to it by moving
    public void insertAt(Direction direction, TraversableNode node)
    {
        movableNodes[DirectionMethods.directionToArrayIndex(direction)] = node;
    }
    
    //Returns an empty node
    public static TraversableNode getEmptyNode()
    {
        return new TraversableNode(Vector3.zero, new GridCoordinate(0, 0), false);
    }


}
