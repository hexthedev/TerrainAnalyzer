using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
	UP, UPRIGHT, RIGHT, DOWNRIGHT, DOWN, DOWNLEFT, LEFT, UPLEFT
}

public static class DirectionMethods
{
    static GridCoordinate up = new GridCoordinate(0, 1);
    static GridCoordinate upright = new GridCoordinate(1, 1);
    static GridCoordinate right = new GridCoordinate(1, 0);
    static GridCoordinate downright = new GridCoordinate(1, -1);
    static GridCoordinate down = new GridCoordinate(0, -1);
    static GridCoordinate downleft = new GridCoordinate(-1, -1);
    static GridCoordinate left = new GridCoordinate(-1, 0);
    static GridCoordinate upleft = new GridCoordinate(-1, 1);

    public static Direction gridToDirection(GridCoordinate coordinateDirection)
    {
        if(GridCoordinate.equals(up, coordinateDirection))
        {
            return Direction.UP;
        }

        if (GridCoordinate.equals(upright, coordinateDirection))
        {
            return Direction.UPRIGHT;
        }

        if (GridCoordinate.equals(right, coordinateDirection))
        {
            return Direction.RIGHT;
        }

        if (GridCoordinate.equals(downright, coordinateDirection))
        {
            return Direction.DOWNRIGHT;
        }

        if (GridCoordinate.equals(down, coordinateDirection))
        {
            return Direction.DOWN;
        }

        if (GridCoordinate.equals(downleft, coordinateDirection))
        {
            return Direction.DOWNLEFT;
        }

        if (GridCoordinate.equals(left, coordinateDirection))
        {
            return Direction.LEFT;
        }

       else
        {
            return Direction.UPLEFT;
        }
    }

    public static GridCoordinate directionToGrid(Direction Direction)
    {
        if (Direction == Direction.UP)
        {
            return up;
        }
        if (Direction == Direction.UPRIGHT)
        {
            return upright;
        }
        if (Direction == Direction.RIGHT)
        {
            return right;
        }
        if (Direction == Direction.DOWNRIGHT)
        {
            return downright;
        }
        if (Direction == Direction.DOWN)
        {
            return down;
        }
        if (Direction == Direction.DOWNLEFT)
        {
            return downleft;
        }
        if (Direction == Direction.LEFT)
        {
            return left;
        }
        else
        {
            return upleft;
        }
    }

    public static int directionToArrayIndex(Direction Direction)
    {
        if (Direction == Direction.UP)
        {
            return 0;
        }
        if (Direction == Direction.UPRIGHT)
        {
            return 1;
        }
        if (Direction == Direction.RIGHT)
        {
            return 2;
        }
        if (Direction == Direction.DOWNRIGHT)
        {
            return 3;
        }
        if (Direction == Direction.DOWN)
        {
            return 4;
        }
        if (Direction == Direction.DOWNLEFT)
        {
            return 5;
        }
        if (Direction == Direction.LEFT)
        {
            return 6;
        }
        else
        {
            return 7;
        }
    }
}
