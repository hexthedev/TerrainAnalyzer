using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisNode {

    private Vector3 position;
    private float radius;
    private Dictionary<string, IAttribute> attributes;
    private List<AnalysisNode> neighbours;

    public AnalysisNode( Vector3 pPosition, float pRadius )
    {
        position = pPosition;
        radius = pRadius;
        attributes = new Dictionary<string, IAttribute>();
        neighbours = new List<AnalysisNode>();
    }

    public void addNeighbour(AnalysisNode neighbour)
    {
        neighbours.Add(neighbour);
    }

    public void addAttribute(IAttribute attribute)
    {
        attribute.calculate(this);
        attributes.Add(attribute.getId(), attribute);
    }

    public IAttribute getAttribute(string id)
    {
        return attributes[id];
    }

    public void recalculateAttribute(string id)
    {
        attributes[id].calculate(this);
    }

    public List<AnalysisNode> getNeighbours()
    {
        return neighbours;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public float getRadius()
    {
        return radius;
    }
    
}
