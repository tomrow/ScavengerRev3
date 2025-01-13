using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipLineNode : MonoBehaviour
{
    [SerializeField, Tooltip("Spawn this when game starts. This should be the grip that the character holds on to to use the zipline.")] GameObject zipLineGripTemplate;
    public Transform nextNode;
    LineRenderer lineRenderer;
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        //if (zipLineGripTemplate != null) 
        //{
        //    newGrip = Instantiate(zipLineGripTemplate, transform.position, transform.rotation) as GameObject; 
        //    newGrip.transform.LookAt(nextNode.transform.position);
        //    data = newGrip.GetComponent<ZipLineGrip>();
        //    data.initialNode = this.gameObject;
        //}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        if (nextNode != null)
        { lineRenderer.SetPosition(1, nextNode.position); }
    }
}
