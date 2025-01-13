using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//using static UnityEditor.Progress;

public class Interactor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity)) return;

        if (! hit.transform.TryGetComponent(out CreateObject objectSpawner)) return;

        Debug.Log("interacting");

        if (    Input.GetAxis("XRI_Right_Trigger")>0.8f || Input.GetAxis("XRI_Left_Trigger") > 0.8f  )
        {
            objectSpawner.SpawnObject();
        }
    }
}
