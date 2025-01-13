using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishWhenFacing : MonoBehaviour
{
    public GameObject SmokePoof;
    public float lookTimerThreshold;
    [SerializeField] Vector3 directionToCameraVector;
    [SerializeField] float directionToCameraDegrees;
    [SerializeField] Vector3 cameraRotation;
    float lookTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        directionToCameraVector = transform.position- Camera.main.transform.position;
        directionToCameraDegrees = Mathf.Atan2(directionToCameraVector.x, directionToCameraVector.z) * Mathf.Rad2Deg;
        cameraRotation = Camera.main.transform.eulerAngles;
        while (directionToCameraDegrees < 0) { directionToCameraDegrees += 360; } //positive only please
        if (WithinRangeAngle(directionToCameraDegrees, cameraRotation.y, 25) && directionToCameraVector.magnitude < 35)
        { lookTimer += Time.fixedDeltaTime; }
        else { lookTimer = 0; };
        if(lookTimer> lookTimerThreshold)
        { Instantiate(SmokePoof, transform.position, Quaternion.identity); transform.position += (Vector3.up * 3000); }
        

    }
    bool WithinRangeAngle(float expected, float actual, float error)
    { 
        if ( (actual > (expected-error))&&(actual < (expected+error)) ) { return true; }
        actual += 360;
        if ((actual > (expected - error)) && (actual < (expected + error))) { return true; }
        actual -= 720;
        if ((actual > (expected - error)) && (actual < (expected + error))) { return true; }
        return false;
    }
}
