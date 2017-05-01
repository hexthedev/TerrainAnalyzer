using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GridSquareCalculator<T> {
    T getValue(Vector3 position);
}
