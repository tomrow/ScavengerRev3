using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonBaseCustom : MonoBehaviour
    
{
    MeshCollider myassets;
    // Start is called before the first frame update
    void Start()
    {
        myassets = GetComponent<MeshCollider>();
        myassets.isTrigger = true;
    }

    // Update is called once per frame.
    void Update()
    {
        
    }
}
