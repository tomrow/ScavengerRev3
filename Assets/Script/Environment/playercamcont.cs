using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercamcont : MonoBehaviour
{
    public Rigidbody cubethrown;
    float velocity = 20f;

    // Start is called before the first frame update.
    void Start()
    {
       /*Rigidbody cubes = Instantiate(cubethrown, new Vector3 (0,0,0), Quaternion.identity);
        Destroy(cubethrown);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Rigidbody cubes = Instantiate(cubethrown,transform.position , Quaternion.Euler (0,0,0));
            cubes.velocity = transform.forward * velocity;
        }
    }
}
