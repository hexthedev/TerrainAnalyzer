using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiFinalizer : MonoBehaviour
{
    public void corrdiorPrune(List<VoronoiNode> workingVoronoi, float corrdiorPercentage, float corridorConstant, float maxRegionalHeightDifference)
    {
        List<VoronoiNode> toWorkWith = new List<VoronoiNode>();
        copyList(workingVoronoi, toWorkWith);
        
        while(toWorkWith.Count > 0)
        {
            if(toWorkWith[0].getNeighbours().Count == 2)
            {
                int isCooridorCount = 0;

                foreach(VoronoiNode node in toWorkWith[0].getNeighbours())
                {
                    if(!blockedByHeight(toWorkWith[0], node, maxRegionalHeightDifference) &&   Mathf.Abs(node.getRadius()-toWorkWith[0].getRadius()) < toWorkWith[0].getRadius()*corrdiorPercentage + corridorConstant)
                    {
                        isCooridorCount += 1;
                    } else
                    {
                        break;
                    }
                }

                if(isCooridorCount == 2)
                {
                    linkRemoveByEdges(toWorkWith[0]);
                    workingVoronoi.Remove(toWorkWith[0]);
                }

            }

            toWorkWith.RemoveAt(0);
        }

    }

    public void triangleCull(List<VoronoiNode> workingVoronoi) 
    {
        List<VoronoiNode> toWorkWith = new List<VoronoiNode>();
        copyList(workingVoronoi, toWorkWith);
        toWorkWith.Sort(new nodeDegreeComparer());
        
        while (toWorkWith.Count > 0)
        {
            VoronoiNode masterNode = toWorkWith[0];
            toWorkWith.RemoveAt(0);

            List<VoronoiNode> removeTriangles = new List<VoronoiNode>();
            copyList(masterNode.getNeighbours(), removeTriangles);
                       

            while(removeTriangles.Count > 0)
            {
                bool hasTriangle = false;
                List<VoronoiNode> triangle = new List<VoronoiNode>();
                triangle.Add(masterNode);

                foreach (VoronoiNode node in masterNode.getNeighbours())
                {
                    if (removeTriangles[0].getNeighbours().Contains(node))
                    {
                        triangle.Add(node);
                        triangle.Add(removeTriangles[0]);

                        hasTriangle = true;
                        break;
                    }
                }
                                
                if (hasTriangle)
                {
                    triangle.Sort(new nodeRadiusComparer());
                    triangle.Reverse();

                    linkRemove(triangle[0], masterNode);
                    workingVoronoi.Remove(triangle[0]);
                }

                removeTriangles.RemoveAt(0);
            }
            
        }
    }

    public void cullBelowRadius(List<VoronoiNode> workingVoronoi, float minradius)
    {
        List<VoronoiNode> toCull = new List<VoronoiNode>();

        foreach (VoronoiNode node in workingVoronoi)
        {
            if(node.getRadius() < minradius)
            {
                if(node.getNeighbours().Count > 0)
                {
                    linkRemove(node, node.getNeighbours()[0]);
                }
                
                toCull.Add(node);
            }
        }

        foreach(VoronoiNode node in toCull)
        {
            workingVoronoi.Remove(node);
        }
    }

    public void largestNodesFirst(List<VoronoiNode> workingVoronoi)
    {
        List<VoronoiNode> toWorkWith = new List<VoronoiNode>();

        foreach(VoronoiNode node in workingVoronoi)
        {
            toWorkWith.Add(node);
        }
        
        toWorkWith.Sort(new nodeRadiusComparer());
        
        while(toWorkWith.Count > 0)
        {
            VoronoiNode masterNode = toWorkWith[0];
            toWorkWith.RemoveAt(0);

            List<VoronoiNode> toCull = new List<VoronoiNode>();

            foreach(VoronoiNode node in toWorkWith)
            {
                if(radiusOverlap(masterNode, node))
                {
                    linkRemove(node, masterNode);

                    toCull.Add(node);
                }
            }

            foreach(VoronoiNode node in toCull)
            {
                workingVoronoi.Remove(node);
                toWorkWith.Remove(node);
            }

        }

    }

    public void radiusMerge( List<VoronoiNode> workingVoronoi, float maxRegionalHeightDifference )
    {
        List<VoronoiNode> toPrune = new List<VoronoiNode>();

        foreach (VoronoiNode node in workingVoronoi)
        {
            foreach (VoronoiNode neighbour in node.getNeighbours())
            {
                if (radiusIsSmaller(node, neighbour) && !blockedByHeight(node, neighbour, maxRegionalHeightDifference)  && radiusOverlap(node, neighbour))
                {
                    linkRemove(node, neighbour);
                    toPrune.Add(node);
                    break;
                }
            }
        }

        prune(workingVoronoi, toPrune);
    }

    public void regionMerge(List<VoronoiNode> workingVoronoi, float maxRegionHeightDifference, float minRegionRadius)
    {
        List<VoronoiNode> toPrune; 

        do
        {
            toPrune = new List<VoronoiNode>();

            foreach (VoronoiNode node in workingVoronoi)
            {
                foreach (VoronoiNode neighbour in node.getNeighbours())
                {
                    if (radiusIsSmaller(node, neighbour) && !blockedByHeight(node, neighbour, maxRegionHeightDifference) && inSameRegion(node, neighbour, minRegionRadius))
                    {
                        linkRemove(node, neighbour);
                        toPrune.Add(node);
                        break;
                    }
                }
            }

            prune(workingVoronoi, toPrune);
        } while (toPrune.Count > 0);
    }

    public void leafPrune(List<VoronoiNode> workingVoronoi, float maxRegionalHeightDifference)
    {
        List<VoronoiNode> toPrune;

        do
        {
            toPrune = new List<VoronoiNode>();

            foreach (VoronoiNode node in workingVoronoi)
            {
                if (node.isLeafNode())
                {
                    foreach (VoronoiNode neighbour in node.getNeighbours())
                    {
                        if (!blockedByHeight(node, neighbour, maxRegionalHeightDifference) && radiusIsSmaller(node, neighbour))
                        {
                            linkRemove(node, neighbour);
                            toPrune.Add(node);
                        }

                    }
                }
                                
            }
            prune(workingVoronoi, toPrune);
        } while (toPrune.Count > 0);
    } 

    public void pruneZeroChild(List<VoronoiNode> workingVoronoi)
    {
        List<VoronoiNode> toPrune = new List<VoronoiNode>();

        foreach(VoronoiNode node in workingVoronoi)
        {
            if (node.getNeighbours().Count == 0)
            {
                toPrune.Add(node);
            }
        }

        foreach(VoronoiNode node in toPrune)
        {
            workingVoronoi.Remove(node);
        }

    }


    #region Helper Functions
    // 1 has smaller radius than 2    
    private bool radiusIsSmaller(VoronoiNode node1, VoronoiNode node2)
    {
        return node1.getRadius() <= node2.getRadius();
    }

    // 1 and 2 have too great a height distance
    private bool blockedByHeight(VoronoiNode node1, VoronoiNode node2, float maxRegionHeightDifference)
    {
        return Mathf.Abs(node1.getHeight() - node2.getHeight()) > maxRegionHeightDifference;
    }

    // The radius of node1 and node2 overlap
    private bool radiusOverlap(VoronoiNode node1, VoronoiNode node2)
    {
        return Vector3.Distance(node1.getPosition(), node2.getPosition()) <= (node1.getRadius() + node2.getRadius());
    }

    // In region
    private bool inSameRegion(VoronoiNode node1, VoronoiNode node2, float minRegionRadius)
    {
        return Vector3.Distance(node1.getPosition(), node2.getPosition()) <= minRegionRadius;
    }

    // remove all in with from toPrune
    private void prune(List<VoronoiNode> toPrune, List<VoronoiNode> with)
    {
        foreach(VoronoiNode node in with)
        {
            toPrune.Remove(node);
        }
    }

    // remove remove from neighbours and link them to by instead
    private void linkRemove(VoronoiNode remove, VoronoiNode by)
    {
        foreach (VoronoiNode neighbour in remove.getNeighbours())
        {
            neighbour.removeNeighbour(remove);

            if (neighbour != by)
            {
                neighbour.addNeighbour(by);
                by.addNeighbour(neighbour);
            }
        }

        remove.removeAllNeighbours();
    }

    // remove remove from neighbours and link all neighbours
    private void linkRemoveByEdges(VoronoiNode remove)
    {
        foreach (VoronoiNode neighbour in remove.getNeighbours())
        {
            foreach(VoronoiNode neighbour2 in remove.getNeighbours())
            {
                if(neighbour != neighbour2)
                {
                    neighbour.addNeighbour(neighbour2);
                    neighbour2.addNeighbour(neighbour);

                    neighbour2.removeNeighbour(remove);
                }
            }

            neighbour.removeNeighbour(remove);
        }

        remove.removeAllNeighbours();
    }

    //Copy and list from, to
    private void copyList(List<VoronoiNode> from, List<VoronoiNode> to)
    {
        foreach(VoronoiNode node in from)
        {
            to.Add(node);
        }
    }

    #endregion















}
