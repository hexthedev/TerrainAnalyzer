using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillCalculator : MonoBehaviour {

    private Terrain terrain;
    public float maxHeight;

    public void getTerrain()
    {
        terrain = gameObject.GetComponent<Terrain>();
    }


    public float calculateHeight(Vector3 position)
    {
        return terrain.SampleHeight(position);
    }

    public float getMaxHeight()
    {
        return maxHeight;
    }

}
