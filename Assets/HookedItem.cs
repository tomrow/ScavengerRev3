using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookedItem : MonoBehaviour
{
    public float weight;
    public CableConnector towParent;
    public Collider c;
    AudioSource scrape;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<Collider>();
        scrape = GetComponent<AudioSource>();
        scrape.Play();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (towParent.hooked == false)
            { towParent = null; }
            c.enabled = towParent == null;
            if (towParent)
            { scrape.volume = towParent.distance.magnitude / 8; }
        }
        catch (NullReferenceException x) 
        { 
            scrape.volume = 0;
            towParent = null;
        }
    }
    
}
