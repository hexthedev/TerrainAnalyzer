using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenTools.Mathematics;

public class Analyzer {

    private float[,] grid;

    private float gridXSpacing;
    private float gridYSpacing;

    private float maxReachableHeight;
    public float maxTraversableSlope;

    private List<TraversableNode> traversabilityMap = new List<TraversableNode>();
    private TraversableNode[,] traversabilityGrid;

    private List<TraversableNode> boarderMap = new List<TraversableNode>();
    private List<VEdge> VEdgeList = new List<VEdge>();

    public Analyzer(float[,] pGrid, float pGridXSpacing, float pGridYSpacing, float pMaxHeight, float pMaxTraversableSlope)
    {
        grid = pGrid;
        gridXSpacing = pGridXSpacing;
        gridYSpacing = pGridYSpacing;
        maxReachableHeight = pMaxHeight;
        maxTraversableSlope = pMaxTraversableSlope;
        makeTraversabilityMap();
        makeBoarderMap();
        makeVoronoiGraph();
    }
    
    // traversabilityMap methods
    private void makeTraversabilityMap()
    {
        traversabilityGrid = new TraversableNode[grid.GetLength(0), grid.GetLength(1)];

        for (int x = 0; x<grid.GetLength(0); x++)
        {
            for(int y = 0; y<grid.GetLength(1); y++)
            {
                if (grid[x,y] >= maxReachableHeight)
                {
                } else
                {
                    TraversableNode toAdd = new TraversableNode(new Vector3(gridIntToWorldfloatX(x), grid[x, y], gridIntToWorldfloatY(y)), new GridCoordinate(x, y), true);
                    traversabilityMap.Add(toAdd);
                    traversabilityGrid[x, y] = toAdd;
                }                
            }
        }

        foreach(TraversableNode node in traversabilityMap)
        {
            GridCoordinate nodeCoordinate = node.getCoordinate();
            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                GridCoordinate directionCoordinate = DirectionMethods.directionToGrid(direction);
                
                if(nodeCoordinate.x + directionCoordinate.x >= grid.GetLength(0) || nodeCoordinate.x + directionCoordinate.x < 0 || nodeCoordinate.y + directionCoordinate.y >= grid.GetLength(1) || nodeCoordinate.y + directionCoordinate.y < 0)
                {
                    continue;
                }
                
                if(grid[nodeCoordinate.x + directionCoordinate.x, nodeCoordinate.y + directionCoordinate.y] < grid[nodeCoordinate.x, nodeCoordinate.y] + maxTraversableSlope)
                {
                    node.insertAt(direction, traversabilityGrid[nodeCoordinate.x + directionCoordinate.x, nodeCoordinate.y + directionCoordinate.y]);
                } else
                {
                    TraversableNode tempNode= traversabilityGrid[nodeCoordinate.x + directionCoordinate.x, nodeCoordinate.y + directionCoordinate.y];
                    
                    if(tempNode != null)
                    {
                        tempNode.setTraversable(false);
                        node.insertAt(direction, tempNode);
                    } else
                    {
                        node.insertAt(direction, TraversableNode.getEmptyNode());
                    }

                }
            }
        }

        List<TraversableNode> tempList = new List<TraversableNode>();

        foreach (TraversableNode node in traversabilityMap)
        {
            if (!node.isTraversable())
            {
                tempList.Add(node);
            }            
        }

        foreach(TraversableNode node in tempList)
        {
            traversabilityMap.Remove(node);
        }
        
    }


    // boarderMap methods
    private void makeBoarderMap()
    {
        foreach (TraversableNode node in traversabilityMap)
        {
            bool isBoarder = false;

            foreach(Direction dir in System.Enum.GetValues(typeof(Direction)))
            {
                if(node.getNeighbour(dir) == null || !node.getNeighbour(dir).isTraversable())
                {
                    isBoarder = true;
                    break;
                }
            }

            if (isBoarder)
            {
                boarderMap.Add(node);
            }
        }
    }

    // Voronoi methods
    private void makeVoronoiGraph()
    {
    
        List<Vector> VoronoiVectors = new List<Vector>();

        foreach (TraversableNode node in boarderMap)
        {
            VoronoiVectors.Add(new Vector(node.getCoordinate().x, node.getCoordinate().y));
        }

        VoronoiGraph nonPrunedGraph = Fortune.ComputeVoronoiGraph(VoronoiVectors);

        List<VoronoiEdge> toPrune = new List<VoronoiEdge>();

        VoronoiGraph voronoiGraph = nonPrunedGraph;

        foreach(VoronoiEdge edge in voronoiGraph.Edges)
        {
            VEdgeList.Add(new VEdge(
                   new Vector3((float)edge.VVertexA[0]*gridXSpacing + gridXSpacing/2, 0, (float)edge.VVertexA[1] * gridYSpacing + gridYSpacing / 2), 
                   new Vector3((float)edge.VVertexB[0] * gridXSpacing + gridXSpacing / 2, 0, (float)edge.VVertexB[1] * gridYSpacing + gridYSpacing / 2)));
        }
    } 

    public List<TraversableNode> getTraversabilityMap()
    {
        return traversabilityMap;
    }

    public List<TraversableNode> getBoarderMap()
    {
        return boarderMap;
    }

    public List<VEdge> getVEdgeList()
    {
        return VEdgeList;
    }

    // WorldGrid Translation Methods
    private float gridIntToWorldfloatX(int a)
    {
        return a * gridXSpacing + gridXSpacing / 2;
    }

    private float gridIntToWorldfloatY(int a)
    {
        return a * gridYSpacing + gridYSpacing / 2;
    }
}
