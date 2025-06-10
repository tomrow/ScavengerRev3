using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitCameraZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement get = (other.gameObject.GetComponent<PlayerMovement>());
        if (get != null)
        { 
            if (get.currentCameraMode != PlayerMovement.CameraMode.DoNothing)
            { get.currentCameraMode = PlayerMovement.CameraMode.ZipToOrbit; }
            get.orbitObject = gameObject.GetComponent<CapsuleCollider>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerMovement get = (other.gameObject.GetComponent<PlayerMovement>());
        if (get != null)
        {
            if (get.currentCameraMode == PlayerMovement.CameraMode.Orbit)
            { get.currentCameraMode = PlayerMovement.CameraMode.ChaseSlow; }
        }
    }
}
