using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowCube : MonoBehaviour
{
    public Rigidbody cubethrown;
    float velocity = 10f;
    bool lastVal;

    // Start is called before the first frame update
    void Start()
    {
        /*Rigidbody cubes = Instantiate(cubethrown, new Vector3 (0,0,0), Quaternion.identity);
         Destroy(cubethrown);*/
        lastVal = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetAxis("XRI_Right_Trigger") > 0.8f) && lastVal == false)
        {
            Rigidbody cubes = Instantiate(cubethrown, transform.position, Quaternion.Euler(0, 0, 0));
            cubes.velocity = transform.forward * velocity;
        }
        lastVal = (Input.GetAxis("XRI_Right_Trigger") > 0.8f);
    }
}
