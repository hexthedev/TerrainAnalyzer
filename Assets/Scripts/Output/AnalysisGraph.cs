using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisGraph {

    private List<AnalysisNode> graph;

    public AnalysisGraph()
    {
        graph = new List<AnalysisNode>();
    }

    public List<AnalysisNode> getGraph()
    {
        return graph;
    }

    public void addNode(AnalysisNode node)
    {
        graph.Add(node);
    }

    public void addAttribute(IAttribute attribute)
    {
        foreach(AnalysisNode node in graph)
        {
            node.addAttribute(attribute.clone());
        }
    }
}
