using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapDown : MonoBehaviour
{
    RaycastHit touchRay;
    // Start is called before the first frame update
    void Start()
    {

        if (Physics.Raycast(transform.position, Vector3.up * -1, out touchRay, transform.localScale.y * 0.6f, 1))
        { transform.position = touchRay.point; }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
