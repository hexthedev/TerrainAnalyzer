using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttribute {
    void calculate(AnalysisNode toCalculate);
    IAttribute clone();
    string getId();
    float getValue();
}
