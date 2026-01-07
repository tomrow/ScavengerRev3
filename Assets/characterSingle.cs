using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterSingle : MonoBehaviour
{
    public GameObject Character;
    public Transform cameraT;
    PlayerMovement phys;
    public PlayerMovement.CameraMode currentCameraMode;
    // Start is called before the first frame update
    void Start()
    {
        Character = gameObject;
        phys = gameObject.GetComponent<PlayerMovement>();
        phys.currentCameraMode = currentCameraMode;
        //phys.cameraLooksAtCharacter = true;
    }

    // Update is called once per frame
    void Update()
    {
        phys.Horizontal = Input.GetAxis("Horizontal"); //pipe controller input into active player
        phys.Vertical = Input.GetAxis("Vertical"); //pipe controller input into active player
        phys.Fire1 = Input.GetAxis("Fire1"); //pipe controller input into active player
        phys.Fire2 = Input.GetAxis("Fire2"); //pipe controller input into active player
        phys.RightStickHorizontal = Input.GetAxis("RightStickHorizontal"); //pipe controller input into active player
        phys.RightStickVertical = Input.GetAxis("RightStickVertical"); //pipe controller input into active player }
    }
}
