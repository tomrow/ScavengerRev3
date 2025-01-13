using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainTreeDrawDist : MonoBehaviour
{
    Terrain terrain;
    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.treeDistance = 40000;
        terrain.treeBillboardDistance = 200;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
