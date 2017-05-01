using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastHeight : MonoBehaviour, GridSquareCalculator<float> {

    public float ground;
    public float height;

    public float getValue(Vector3 position)
    {
        RaycastHit hit;
        Physics.Raycast(position + new Vector3(0, height, 0), Vector3.down, out hit);

        return height - hit.distance + ground;
    }

    public void setTerrain()
    {
        throw new NotImplementedException();
    }
}
