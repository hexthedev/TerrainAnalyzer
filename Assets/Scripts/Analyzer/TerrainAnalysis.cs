using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenTools.Mathematics;
using System.Diagnostics;


public class TerrainAnalysis : MonoBehaviour {

    #region Control Variables
    [Header("Gizmos")]
    public bool seeTraverabilityMap;
    public bool seeBoarderMap;
    public bool seeVeronoi;
    public bool seeRadius;
    public bool seeAnalysis;
    public bool seeAttribute;
    public string attribute;
    
    [Header("World Distance")]
    public int terrainX;
    public int terrainY;
    
    [Header("Percent of Real Space")]
    [Range(0.1f, 4)]
    public float gridXPercentage;
    [Range(0.1f, 4)]
    public float gridYPercentage;

    [Header("Map Preferences")]
    public float maxReachableHeight;
    public float maxTraversableSlope;

    [Header("Radius Computation")]
    public float iterationDistance;
    public int numOfDirections;

    [Header("Pruning Settings")]
    public float minNodeRadius;
    public float minRegionRadius;
    public float maxRegionHeightDifference;
    public float corridorPercentage;
    public float corridorConstant;

    //Domain of Grid
    private float terrainXGrid;
    private float terrainYGrid;

    //Real World Space represented by grid line
    private float gridXSpacing;
    private float gridYSpacing;
    #endregion
    
    private VoronoiFinalizer veronoiFinalizer;
    private Terrain terrain; 
    private float[,] grid;
    private Analyzer analyzer;
    private List<VoronoiNode> workingVoronoi = new List<VoronoiNode>();
    private AnalysisGraph analysisGraph;

    // Use this for initialization
    void Start () {

        #region Basic Initialization
        terrain = gameObject.GetComponent<Terrain>();
        maxTraversableSlope *= Mathf.Min(gridXPercentage, gridYPercentage);

        terrainXGrid = 100f / gridXPercentage;
        terrainYGrid = 100f / gridYPercentage;

        gridXSpacing = terrainX / 100f * gridXPercentage;
        gridYSpacing = terrainY / 100f * gridYPercentage;
        #endregion

        #region Algorithm
        //Create grid of heights        
        grid = new float[(int)terrainXGrid, (int)terrainYGrid];

        for (int x = 0; x < terrainXGrid; x++)
        {
            for (int y = 0; y < terrainYGrid; y++)
            {
                grid[x, y] = terrain.SampleHeight(new Vector3(x * gridXSpacing + gridXSpacing / 2f, 0, y * gridYSpacing + gridYSpacing / 2f));
            }
        }
        
        //Create a new analyzer
        analyzer = new Analyzer(grid, gridXSpacing, gridYSpacing, maxReachableHeight, maxTraversableSlope);
        
        //Convert a pruned Voronoi to VoronoiGraph        
        convertToVoronoiGraph(pruneVoronoi(analyzer.getVEdgeList()));
        
        //Calculate the radius of each node
        foreach (VoronoiNode node in workingVoronoi)
        {
            node.setRadius(computeRadius(node));
        }
        
        //Perform culling algorithms
        cullVeronoi();
        #endregion

        //Set up Analysis Graph
        analysisGraph = getAnalysisGraph();


        //ROB!!
        /*
         * Calling addAttribute() lets you put in an interface of type IAttribute. I have given you two examples below. 
         * 
         * ChokeValue's constructor ONLY takes a string because caluations to get choke value can be done without calling unity functions. 
         * 
         * UpHill's constructor takes a string and a HillCalculator. It's annoying, but this is necessary because it requires MonoBehaviour to do it's calculations. 
         * Whenever you're trying to make an attribute that requires extra infomation from the gameworld, you need to make it like UpHill. Basically you need another
         * MonoBehaviour Object (Like hill Calculator), and you need to attach it to the terrain you'r analyzing. Again this is annoying but it's necessary because
         * of how unity objects work. Take a look at upHill and you should see how it's working. 
         */


        analysisGraph.addAttribute(new ChokeValue("choke"));

        HillCalculator calc = gameObject.GetComponent<HillCalculator>();
        calc.getTerrain();
        analysisGraph.addAttribute(new UpHill("hill", calc));
        
    }

    #region Analysis Graph Algorithms
    public AnalysisGraph getAnalysisGraph()
    {
        AnalysisGraph toReturn = new AnalysisGraph();

        Dictionary<Vector3, AnalysisNode> tempIndex = new Dictionary<Vector3, AnalysisNode>();

        foreach(VoronoiNode node in workingVoronoi)
        {
            AnalysisNode temp = new AnalysisNode(node.getPosition(), node.getRadius());
            toReturn.addNode(temp);
            tempIndex.Add(node.getPosition(), temp);
        }

        foreach(VoronoiNode node in workingVoronoi)
        {
            AnalysisNode temp = tempIndex[node.getPosition()];

            foreach(VoronoiNode neighbour in node.getNeighbours())
            {
                if (tempIndex.ContainsKey(neighbour.getPosition()))
                { 
                    temp.addNeighbour(tempIndex[neighbour.getPosition()]);
                }

            }
        }
                
        return toReturn;
    }
    #endregion

    #region VeronoiGraph Algorithms
    private void cullVeronoi()
    {
                
        veronoiFinalizer = gameObject.GetComponent<VoronoiFinalizer>();
        
        veronoiFinalizer.cullBelowRadius(workingVoronoi, minNodeRadius);
        veronoiFinalizer.largestNodesFirst(workingVoronoi);
        veronoiFinalizer.regionMerge(workingVoronoi, maxRegionHeightDifference, minRegionRadius);
        veronoiFinalizer.triangleCull(workingVoronoi);
        veronoiFinalizer.leafPrune(workingVoronoi, maxRegionHeightDifference);
        veronoiFinalizer.corrdiorPrune(workingVoronoi, corridorPercentage, corridorConstant, maxRegionHeightDifference);
        veronoiFinalizer.pruneZeroChild(workingVoronoi);
    }


    private float computeRadius(VoronoiNode node)
    {
        //Create Direction vectorList
        List<Vector3> directionVectors = new List<Vector3>();

        float angleChecks = 360f / numOfDirections;

        for(float i = 0; i<360; i += angleChecks)
        {
            directionVectors.Add( (Quaternion.AngleAxis(i, Vector3.up) * new Vector3(1, 0, 0)));
        }

        //Create Height Dictionary
        Dictionary<Vector3, float> heightCounter = new Dictionary<Vector3, float>();
        float initialHeight = terrain.SampleHeight(node.getPosition());

        foreach(Vector3 vector in directionVectors)
        {
            heightCounter.Add(vector, initialHeight);
        }

        //Compute
        Vector3 initialPosition = node.getPosition();
        for (float i = iterationDistance; i<Mathf.Max(terrainX, terrainY); i += iterationDistance)
        {
            foreach(Vector3 direction in directionVectors)
            {
                Vector3 positionChecking = initialPosition + (direction * i);

                if(positionChecking.x > terrainX || positionChecking.x < 0 || positionChecking.y > terrainY || positionChecking.y < 0)
                {
                    return i;
                }

                float currentSample = terrain.SampleHeight(positionChecking);
                if ( Mathf.Abs(currentSample-heightCounter[direction]) > maxTraversableSlope * (iterationDistance/Mathf.Min(gridXSpacing, gridYSpacing))){
                    return i;
                }

                heightCounter[direction] = currentSample;
            }
        }

        return 0;
    }

    private void convertToVoronoiGraph(List<VEdge> edges)
    {
        Dictionary<Vector3, VoronoiNode> tempNodes = new Dictionary<Vector3, VoronoiNode>();

        foreach (VEdge edge in edges)
        {
            
            if (!tempNodes.ContainsKey(edge.point1))
            {
                tempNodes.Add(edge.point1, new VoronoiNode(edge.point1));
            }

            if (!tempNodes.ContainsKey(edge.point2))
            {
                tempNodes.Add(edge.point2, new VoronoiNode(edge.point2));
            }
            
            tempNodes[edge.point1].addNeighbour(tempNodes[edge.point2]);
            tempNodes[edge.point2].addNeighbour(tempNodes[edge.point1]);
        }

        foreach(VoronoiNode node in tempNodes.Values)
        {
            node.setHeight(terrain.SampleHeight(node.getPosition()));
            workingVoronoi.Add(node);
        }

    }

    private List<VEdge> pruneVoronoi(List<VEdge> unprunedGraph)
    {
        List<VEdge> toPrune = new List<VEdge>();

        foreach (VEdge edge in unprunedGraph)
        {
            if (edge.point1.x > terrainX || edge.point1.x < 0 || edge.point1.z > terrainY || edge.point1.z < 0)
            {
                toPrune.Add(edge);
            }

            if (edge.point2.x > terrainX || edge.point2.x < 0 || edge.point2.z > terrainY || edge.point2.z < 0)
            {
                toPrune.Add(edge);
            }

            if (terrain.SampleHeight(edge.point1) >= maxReachableHeight || terrain.SampleHeight(edge.point2) >= maxReachableHeight)
            {
                toPrune.Add(edge);
            }

            if ( Mathf.Abs(terrain.SampleHeight(edge.point1) - terrain.SampleHeight(edge.point2)) > maxTraversableSlope )
            {
                toPrune.Add(edge);
            }

        }

        foreach (VEdge edge in toPrune)
        {
            unprunedGraph.Remove(edge);
        }
        
        foreach (VEdge edge in unprunedGraph)
        {
            edge.point1.y = terrain.SampleHeight(edge.point1)+2;
            edge.point2.y = terrain.SampleHeight(edge.point2)+2;
        }

        return unprunedGraph;
    }
    #endregion

    #region Helper functions
    private void removeAllFrom(List<VoronoiNode> all, List<VoronoiNode> from)
    {
        foreach(VoronoiNode node in all)
        {
            from.Remove(node);
        }
    }
    #endregion

    #region GIZMOS    
    private void OnDrawGizmosSelected()
    {
        if (seeTraverabilityMap)
        {
            gizmosTraversabilityMapTraversableNodesAndEdges(analyzer);
        }

        if (seeBoarderMap)
        {
            gizmosBoarderMapNodesAndEdges(analyzer);
        }

        if (seeVeronoi)
        {
            gizmosPrintWorkingVoronoi();
        }

        if (seeRadius)
        {
            gizmosPrintWorkingVoronoiRadius();
        }

        if (seeAnalysis)
        {
            gizmosPrintAnalysisGraph(analysisGraph);
        }

        if (seeAttribute)
        {
            gizmosPrintAnalysisGraphWithAttribute(analysisGraph, attribute);
        }
    }


    //Test Methods
    private void gizmosTraversabilityMapNodes(Analyzer a)
    {
        foreach(TraversableNode x in a.getTraversabilityMap())
        {
            Gizmos.DrawSphere(x.getPosition(), 0.5f);
        }
    }

    private void gizmosTraversabilityMapTraversableNodes(Analyzer a)
    {
        foreach (TraversableNode x in a.getTraversabilityMap())
        {
            if (x.isTraversable())
            {
                Gizmos.DrawSphere(x.getPosition(), 0.5f);
            }
        }
    }

    private void gizmosTraversabilityMapTraversableNodesAndEdges(Analyzer a)
    {
        foreach (TraversableNode x in a.getTraversabilityMap())
        {
            if (x.isTraversable())
            {
                Gizmos.DrawSphere(x.getPosition(), 0.5f);

                foreach(Direction dir in System.Enum.GetValues(typeof(Direction)))
                {
                    TraversableNode temp = x.getNeighbour(dir);
                    if (temp != null && temp.isTraversable())
                    {
                        Gizmos.DrawLine(temp.getPosition(), x.getPosition());
                    }
                }
            }
        }
    }

    private void gizmosBoarderMapNodesAndEdges(Analyzer a)
    {
        foreach(TraversableNode node in a.getBoarderMap())
        {
            Gizmos.DrawSphere(node.getPosition(), 3f);

        }
    }

    private void gizmosVoronoiGraphEdges(List<VEdge> edges)
    {
        foreach (VEdge edge in edges)
        {
            Gizmos.DrawLine(edge.point1, edge.point2);
        }
    }

    private void gizmosPrintWorkingVoronoi()
    {
        foreach (VoronoiNode node in workingVoronoi)
        {
            //Gizmos.DrawSphere(node.getPosition(), 8f);
            foreach (VoronoiNode child in node.getNeighbours())
            {
                Gizmos.DrawLine(node.getPosition(), child.getPosition());
            }
        }
    }

    private void gizmosPrintWorkingVoronoiRadius()
    {
        foreach (VoronoiNode node in workingVoronoi)
        {
            Gizmos.DrawSphere(node.getPosition(), node.getRadius());
        }
    }

    private void gizmosPrintWorkingVoronoiWithChildren(int n)
    {
        foreach (VoronoiNode node in workingVoronoi)
        {
            if(node.getNeighbours().Count == n)
            {
                Gizmos.DrawSphere(node.getPosition(), 2f);

                foreach (VoronoiNode child in node.getNeighbours())
                {

                    Gizmos.DrawLine(node.getPosition(), child.getPosition());
                }
            }            
        }
    }

    private void gizmosPrintAnalysisGraph(AnalysisGraph a)
    {
        foreach (AnalysisNode node in a.getGraph())
        {
            Gizmos.DrawSphere(node.getPosition(), 10f);
            foreach (AnalysisNode node2 in node.getNeighbours())
            {
                Gizmos.DrawLine(node.getPosition(), node2.getPosition());
            }
        }
    }


    private void gizmosPrintAnalysisGraphWithAttribute(AnalysisGraph a, string attributeId)
    {
        foreach (AnalysisNode node in a.getGraph())
        {
            Gizmos.DrawSphere(node.getPosition(), node.getAttribute(attributeId).getValue()*30);
            
            foreach (AnalysisNode node2 in node.getNeighbours())
            {
                Gizmos.DrawLine(node.getPosition(), node2.getPosition());
            }
        }
    }
    #endregion
}
