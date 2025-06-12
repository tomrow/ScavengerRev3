using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shotgun : MonoBehaviour
{
    public GameObject platform;
    public Transform robot;
    float counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        robot = GameObject.Find("robot").transform;
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
        if (other.gameObject.tag == "blobDisabled")
        {
            GameObject b = Instantiate(platform, other.gameObject.transform.position, Quaternion.identity);
            b.transform.position = new Vector3(b.transform.position.x, transform.position.y, b.transform.position.z);
            Destroy(other.gameObject);
            
        }
        if (other.gameObject.GetComponent<movingPlatform>())
        {
            GameObject b = Instantiate(platform, other.gameObject.transform.position, Quaternion.identity);
            b.transform.parent = robot;
            b.transform.position = new Vector3(b.transform.position.x, transform.position.y, b.transform.position.z);
            for (int i = 0; i < 10; i++)
            { 
                b = Instantiate(platform, other.gameObject.transform.position, Quaternion.identity);
                b.transform.parent = robot;
                b.transform.position = new Vector3(b.transform.position.x, transform.position.y, b.transform.position.z);
            }
            Destroy(other.gameObject);

        }
    }
}
