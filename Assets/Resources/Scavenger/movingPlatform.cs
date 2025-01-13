using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    [Tooltip("Also the center point of circle mode")] public Vector3 position1;
    public Vector3 position2;
    public float circleModeOrbitMagnitude;
    public float waitPeriod;
    public float travelTime;
    public enum Modes { Lerp, LerpOneWay, Circle }
    public Modes mode;
    float lerpTimer;
    int lerpSubMode;
    public bool warpBackToPositionOneInOneWayMode;
    float waitTimer;
    Vector3 movementOffsetPerTic;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = position1;
        float speed = Time.fixedDeltaTime / travelTime;
        movementOffsetPerTic = Vector3.zero;
        lerpTimer = 0;
        waitTimer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (mode == Modes.Lerp) 
        {
            if (lerpSubMode == 0) //travel from position 1 to position 2
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer); //mustn't leave these bounds or the player may slide around further than the platform travels
                waitTimer = 0; //set this to zero to prepare for next stage;
                Vector3 newPos = Vector3.Lerp(position1, position2, lerpTimer);
                movementOffsetPerTic = newPos - transform.position;
                transform.position = newPos;
                lerpSubMode += lerpTimer >= 1 ? 1 : 0; //when the lerpTimer is 1, move on to the next stage
            }
            if(lerpSubMode == 1) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero; //set this to zero so the player doesnt slide around
                lerpSubMode += waitTimer >= waitPeriod ? 1 : 0;
            }
            if (lerpSubMode == 2) //travel from position 2 to position 1
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer); //mustn't leave these bounds or the player may slide around further than the platform travels
                waitTimer = 0; //set this to zero to prepare for next stage;
                Vector3 newPos = Vector3.Lerp(position2, position1, lerpTimer);
                movementOffsetPerTic = newPos - transform.position;
                transform.position = newPos;
                lerpSubMode += lerpTimer >= 1 ? 1 : 0; //when the lerpTimer is 1, move on to the next stage
            }
            if (lerpSubMode == 3) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero;
                if (waitTimer >= waitPeriod) { lerpSubMode = 0; }
            }
        }
        if (mode == Modes.LerpOneWay)
        {
            if (lerpSubMode == 0) //travel from position 1 to position 2
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer); //mustn't leave these bounds or the player may slide around further than the platform travels
                waitTimer = 0; //set this to zero to prepare for next stage;
                Vector3 newPos = Vector3.Lerp(position1, position2, lerpTimer);
                movementOffsetPerTic = newPos - transform.position;
                transform.position = newPos;
                lerpSubMode += lerpTimer >= 1 ? 1 : 0; //when the lerpTimer is 1, move on to the next stage
            }
            if (lerpSubMode == 1) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero; //set this to zero so the player doesnt slide around
                lerpSubMode += waitTimer >= waitPeriod ? 1 : 0;
            }
            if (lerpSubMode == 2) //don't move
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer);
                waitTimer = 0;
                //transform.position = Vector3.Lerp(position2, position1, lerpTimer);
                //movementOffsetPerTic = position1 - Vector3.Lerp(position2, position1, Time.fixedDeltaTime / travelTime);
                lerpSubMode += lerpTimer >= 1 ? 1 : 0;
            }
            if (lerpSubMode == 3) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero;
                if (waitTimer >= waitPeriod) 
                {
                    if (warpBackToPositionOneInOneWayMode)
                    { lerpSubMode = 0; }
                    else { waitTimer = 0; }
                }
            }
        }
        if (mode == Modes.Circle)
        {
            if (lerpSubMode == 0) //travel from position 1 to position 2
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer); //mustn't leave these bounds or the player may slide around further than the platform travels
                waitTimer = 0; //set this to zero to prepare for next stage;
                float ofs = Mathf.Deg2Rad * transform.localEulerAngles.y;
                float ang = ofs + (Mathf.PI * lerpTimer);
                ang = ang % (Mathf.PI * 2);
                Vector3 newPos = new Vector3 (Mathf.Sin(ang), 0, Mathf.Cos(ang));
                newPos *= circleModeOrbitMagnitude;
                newPos += position1;
                movementOffsetPerTic = newPos - transform.position;
                transform.position = newPos;
                lerpSubMode += lerpTimer >= 1 ? 1 : 0; //when the lerpTimer is 1, move on to the next stage
            }
            if (lerpSubMode == 1) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero; //set this to zero so the player doesnt slide around
                lerpSubMode += waitTimer >= waitPeriod ? 1 : 0;
            }
            if (lerpSubMode == 2) //travel from position 2 to position 1
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer); //mustn't leave these bounds or the player may slide around further than the platform travels
                waitTimer = 0; //set this to zero to prepare for next stage;
                float ofs = Mathf.Deg2Rad * transform.localEulerAngles.y;
                float ang = ofs + (Mathf.PI * lerpTimer);
                ang += Mathf.PI; //rotate 180 degrees so we dont just warp back to phase 0 start
                Vector3 newPos = new Vector3(Mathf.Sin(ang), 0, Mathf.Cos(ang));
                newPos *= circleModeOrbitMagnitude;
                newPos += position1;
                movementOffsetPerTic = newPos - transform.position;
                transform.position = newPos;
                lerpSubMode += lerpTimer >= 1 ? 1 : 0; //when the lerpTimer is 1, move on to the next stage
            }
            if (lerpSubMode == 3) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero;
                if (waitTimer >= waitPeriod) { lerpSubMode = 0; }
            }
        }
    }
    public Vector3 GetMovementOffsetPerTic()
    { return movementOffsetPerTic; }
}
