using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityTerrainHeight : MonoBehaviour, GridSquareCalculator<float>
{
    private Terrain terrain;

    public float getValue(Vector3 position)
    {
        return terrain.SampleHeight(position);
    }

    public void setTerrain()
    {
        terrain = gameObject.GetComponent<Terrain>();
    }
}
