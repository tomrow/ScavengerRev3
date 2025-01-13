using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//using static UnityEditor.Progress;

public class InteractorRightController : MonoBehaviour
{
    GameObject rightController;
    int clicked;
    // Start is called before the first frame update
    void Start()
    {
        rightController = GameObject.Find("RightCtrlRayOrigin");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Physics.Raycast(rightController.transform.position, rightController.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity)) return;

        if (!hit.transform.TryGetComponent(out CreateObject objectSpawner)) return;

        Debug.Log("interacting");

        if (Input.GetAxis("XRI_Right_Trigger") > 0.8f)
        {
            //objectSpawner.SpawnObject();
            clicked += 1;
            if (clicked > 2) { clicked = 2; }

        }
        else
        { clicked = 0; }
        if (clicked == 1)
        {
            objectSpawner.SpawnObject();
        }
    }
}
