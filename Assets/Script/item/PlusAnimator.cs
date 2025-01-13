using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusAnimator : MonoBehaviour
{
    public Transform mainCamera;
    int frameCount = 0;
    public int frameDuration;
    int CF; //current frame
    Transform bundle;
    MeshRenderer flick0;
    MeshRenderer flick1;
    MeshRenderer flick2;
    MeshRenderer flick3;

    GameObject mainCamObj;
    // Start is called before the first frame update.
    void Start()
    {
        mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = mainCamObj.transform;
        bundle = transform.Find("LookAtCameraBundle");
        flick0 = bundle.Find("f0").gameObject.GetComponent<MeshRenderer>();
        flick1 = bundle.Find("f1").gameObject.GetComponent<MeshRenderer>();
        flick2 = bundle.Find("f2").gameObject.GetComponent<MeshRenderer>();
        flick3 = bundle.Find("f3").gameObject.GetComponent<MeshRenderer>();
    }

    // FixedUpdate is called once per frame, Update is called once per Unity tic (much faster than every displayed frame)
    void FixedUpdate()
    {
        mainCamera = mainCamObj.transform;
        bundle.LookAt(mainCamera, Vector3.up); //always point toward camera
        frameCount += 1;
        frameCount = frameCount % (frameDuration * 4);
        if (((frameCount / frameDuration) > 3) || frameCount < 0)
        {
            frameCount = 0; //frameCount should range from 0 to 3, deviations should be corrected
        }
        else
        {
            CF = Mathf.FloorToInt(frameCount / frameDuration);
            //Debug.Log(CF);
            if (CF == 0)
            {
                flick0.enabled = true;
                flick1.enabled = false;
                flick2.enabled = false;
                flick3.enabled = false;
            }
            else if (CF == 1)
            {
                flick0.enabled = false;
                flick1.enabled = true;
                flick2.enabled = false;
                flick3.enabled = false;
            }
            else if (CF == 2)
            {
                flick0.enabled = false;
                flick1.enabled = false;
                flick2.enabled = true;
                flick3.enabled = false;
            }
            else if (CF == 3)
            {
                flick0.enabled = false;
                flick1.enabled = false;
                flick2.enabled = false;
                flick3.enabled = true;
            }
        }

    }
}
