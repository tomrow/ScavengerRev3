using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class GrowthBillboard : MonoBehaviour
{
    public Transform mainCamera;
    GameObject mainCamObj;
    // Start is called before the first frame update
    void Start()
    {
        mainCamObj = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = mainCamObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera, Vector3.up); //always point toward camera
        transform.Rotate(Vector3.up * 180);
        transform.Rotate(0, 0, UnityEngine.Random.Range(0f, 360f));
        if (UnityEngine.Random.Range(0f,1f)<0.2f)
        { transform.localScale = new Vector3(UnityEngine.Random.Range(0.5f, 1.3f), UnityEngine.Random.Range(0.5f, 1.3f), UnityEngine.Random.Range(0.5f, 1.3f)); }
    }
}
