using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shotgun : MonoBehaviour
{
    public GameObject platform;
    
    float counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > 0.1f) {
            GC.Collect();
            GC.WaitForPendingFinalizers(); 
            Destroy(gameObject); }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "blob")
        {
            GameObject b = Instantiate(platform, other.gameObject.transform.position, Quaternion.identity);
            b.transform.position = new Vector3(b.transform.position.x, transform.position.y, b.transform.position.z);
            Destroy(other.gameObject);
            
        }
    }
}
