using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPlatform : MonoBehaviour
{
    [Tooltip("Items to be consumed are placed in here automatically by a tow hook")] public List<GameObject> consuming;
    [Tooltip("number of items consumed")]public int consumed;
    [Tooltip("number of items needed for the event to trigger")] public int goal;
    [Tooltip("gameobject containing an event")] public GameObject eventObj;
    [Tooltip("location for event")] public Vector3 eventLocation;
    bool used;
    // Start is called before the first frame update
    void Start()
    {
        used = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (consumed >= goal && !used)
        { Instantiate(eventObj, eventLocation, Quaternion.identity); used = true; }
        else 
        {
            if (consuming.Count > 0)
            {
                for(int i = 0; i<consuming.Count;i++)
                {
                    GameObject go = consuming[i];
                    go.transform.localScale -= Vector3.one * (Time.fixedDeltaTime / 2);
                    if (go.transform.localScale.x <= 0 || go.transform.localScale.y <= 0 || go.transform.localScale.z <= 0)
                    {
                        consumed++;
                        Destroy(go);
                        consuming.RemoveAt(i);
                    }
                }
            }
        }
    }
}
