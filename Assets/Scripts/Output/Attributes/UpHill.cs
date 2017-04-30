using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpHill : IAttribute
{

    private string id;
    private float value;

    HillCalculator calculator;
    
    public UpHill(string pId, HillCalculator pCalculator)
    {
        id = pId;
        calculator = pCalculator;
    }    

    public void calculate(AnalysisNode toCalculate)
    {
        float heightDifference = 0;
        float currentHeight = calculator.calculateHeight(toCalculate.getPosition());
                
        foreach(AnalysisNode node in toCalculate.getNeighbours())
        {
            float difference = currentHeight - calculator.calculateHeight(node.getPosition());

            if(heightDifference < difference)
            {
                heightDifference = difference;
            }
        }

        value = heightDifference / calculator.getMaxHeight();
    }

    public IAttribute clone()
    {
        UpHill toReturn = new UpHill(id, calculator);
        toReturn.value = getValue();

        return toReturn;
    }

    public string getId()
    {
        return id;
    }

    public float getValue()
    {
        return value;
    }
}
