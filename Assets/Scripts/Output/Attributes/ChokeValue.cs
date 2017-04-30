using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChokeValue : IAttribute {

    private string  attributeId;
    private float value;
    
    public ChokeValue(string id)
    {
        attributeId = id;
        value = 0;
    }

    public void calculate(AnalysisNode toCalculate)
    {
        float surrowndRadius = 0;

        foreach(AnalysisNode node in toCalculate.getNeighbours())
        {
            surrowndRadius += node.getRadius();
        }

        surrowndRadius += toCalculate.getRadius();

        value =  1 - (toCalculate.getRadius() / surrowndRadius);


    }
    
    public string getId()
    {
        return attributeId;
    }

    public float getValue()
    {
        return value;
    }

    IAttribute IAttribute.clone()
    {
        ChokeValue temp = new ChokeValue(attributeId);
        temp.value = getValue();

        return temp;
    }
}
