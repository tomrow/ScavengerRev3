using System;
using UnityEngine;

public class VrLoco : MonoBehaviour
{
    //public Transform AlanAnimatorHeirarchy;
    public float minSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float speedCap;
    //public bool ultraMode;
    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    public float stickPushedFromCenter;
    public int playerActionMode = 0;
    //0 = walking
    //1 = jumping
    //2 = knockback
    //3 = punching
    //4 = dragging something
    //5 = falling
    //6 = death
    //7 = secret dance
    //8 = jump windup
    [SerializeField] Vector2 inputmag;
    public float gndAccelleration;

    public float gndFriction;

    Transform cameraT;
    Vector2 input2;
    [SerializeField] Vector2 speed2;
    RaycastHit touchRay;
    public float hspeed;

    public int jumpHesitationFrames;

    //public float yangle;
    public float angleRun;

    public float avgPushAngle;
    float normalisedSpeed;
    public Animator animatorMesh;
    public float pushAngle;
    float targetTravelAngleDeg;
    float pushAngleDeg;
    GameObject go_XrOrigin;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Start!!");
        cameraT = Camera.main.transform;
        //Get component collections
        go_XrOrigin = GameObject.FindWithTag("XR Origin");






        //angleRun = 0;
    }

    private void CollideWallTic()
    {

        float checkDist = speed2.magnitude * Time.fixedDeltaTime > 0.3f ? speed2.magnitude * Time.fixedDeltaTime : 0.3f;
        //checkDist = (speed2.magnitude * Time.fixedDeltaTime) * 4;
        checkDist += 0.1f;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 0) * checkDist), Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, -1, 0) * checkDist), Color.green);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, -1) * checkDist), Color.blue);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 0) * checkDist), Color.red);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 1, 0) * checkDist), Color.green);
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * checkDist), Color.blue);
        //Debug.Log(checkDist);
        for (float rot = 0; rot < 2; rot += 0.125f) //increments of 0.125 degrees collision rays
        {
            Vector3 colAngle = new Vector3(Mathf.Sin(rot * Mathf.PI), 0, Mathf.Cos(rot * Mathf.PI));
            if (Physics.Raycast(transform.position, transform.TransformDirection(colAngle), out touchRay, (checkDist * transform.localScale.x), 1))
            {
                //Debug.Log("Ray was cast forward, and we got a hit!");
                transform.position = touchRay.point;
                //Vector3 colnorm = touchRay.normal;
                //colnorm.y = 0;
                transform.Translate(colAngle * (transform.localScale.z * -0.4f), Space.World);
                //speed2.x *= 0.8f;
                //speed2.y *= 0.8f;
            }
        }



    }


    private void MoveCharacter4Tic()
    {
        input2 = new Vector2(Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal"), 0-Input.GetAxis("XRI_Right_Primary2DAxis_Vertical")) * (1-Input.GetAxis("XRI_Right_Grip"));
        Vector2 preinputmag = input2 * runSpeed; //Multiply to reach intended speed values for later math


        float targetTravelAngle = (Mathf.Atan2(input2.x, input2.y)) + (cameraT.eulerAngles.y * Mathf.Deg2Rad); //Get stick angle and add camera angle to it, so that forwards is always forwards relative to camera
        targetTravelAngleDeg = targetTravelAngle * Mathf.Rad2Deg;
        if (input2.magnitude > 0.02)
        {
            if (speed2.magnitude > minSpeed * 6)
            {
                pushAngleDeg = Mathf.SmoothDampAngle(pushAngleDeg, targetTravelAngleDeg, ref turnSmoothVelocity, 0.1f); //Smoothly rotate and limit cornering speed
            }
            else
            {
                pushAngleDeg = targetTravelAngleDeg;
            }
        }
        pushAngle = pushAngleDeg * Mathf.Deg2Rad;  //c# radian nonsense, why cant I set it to degrees for everything?
        Vector2 inputmag = new Vector2(Mathf.Sin(targetTravelAngle), Mathf.Cos(targetTravelAngle));
        inputmag = inputmag.normalized * (preinputmag.magnitude > speedCap ? speedCap : preinputmag.magnitude); //recalculate stick values based on calculated angle
        inputmag.x = (float)(Mathf.Round(inputmag.x * 10000f) / 10000);
        inputmag.y = (float)(Mathf.Round(inputmag.y * 10000f) / 10000); //4 decimal places
        if (Math.Abs(avgPushAngle - pushAngle) > Mathf.PI * 1.2f)
        {
            speed2 = speed2 * 0.5f; //speed braking thing that I'm not sure is doing anything but it doesnt seem to be causing any weirdness so leave it in or delete it if you want
        }
        avgPushAngle = (avgPushAngle + pushAngle) / 2; //mean current angle with the one from last tic

        Vector2 forceAdd = inputmag.normalized * (hspeed * 4);
        if (speed2.magnitude < inputmag.magnitude)
        {
            speed2.x += ((inputmag.x - speed2.x) * gndAccelleration) * Time.fixedDeltaTime;
            speed2.y += ((inputmag.y - speed2.y) * gndAccelleration) * Time.fixedDeltaTime;
        }
        else if (speed2.magnitude > inputmag.magnitude)
        {
            speed2.y *= gndFriction;
            speed2.x *= gndFriction;
        }
        speed2 = new Vector2(Mathf.Sin(pushAngle), Mathf.Cos(pushAngle)) * speed2.magnitude;

        speed2.x += forceAdd.x;
        speed2.y += forceAdd.y; //TODO: for ramps and speed pads, this needs work
        normalisedSpeed = speed2.magnitude;
        speed2 = speed2.normalized * (normalisedSpeed > speedCap ? speedCap : normalisedSpeed); //speed limit
        hspeed = 0;
        if (Math.Abs(speed2.x) < minSpeed)
        {
            speed2.x = 0;
        }
        if (Math.Abs(speed2.y) < minSpeed)
        {
            speed2.y = 0;
        } //eliminate floating point shenanigans so that zero means zero

        stickPushedFromCenter = speed2.magnitude / (runSpeed / 4);
        if (input2.magnitude > 0.06)
        {
            if (speed2.magnitude < 12)
            {
                angleRun = Mathf.Atan2(speed2.x, speed2.y) * Mathf.Rad2Deg;
            }
            else
            {
                angleRun = Mathf.Atan2(speed2.x, speed2.y) * Mathf.Rad2Deg;
            }
        }

        if (!WallColMoveTic()) { transform.Translate((new Vector3(speed2.x, 0, speed2.y) * transform.localScale.x) * Time.fixedDeltaTime, Space.World); }
        
    }





    private bool WallColMoveTic()
    {
        //return false;
        float checkDist = speed2.magnitude * Time.fixedDeltaTime > 0.3f ? speed2.magnitude * Time.fixedDeltaTime : 0.3f;
        checkDist = (speed2.magnitude * Time.fixedDeltaTime);
        //checkDist += 0.1f;
        //checkDist *= 1.5f;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(speed2.normalized.x, 0, speed2.normalized.y)) * (checkDist * transform.localScale.x), Color.red);
        //Debug.Log(checkDist);
        CollideWallTic();
        //Debug.Log(transform.TransformDirection(new Vector3(speed2.normalized.x, 0, speed2.normalized.y)));
        Debug.Log(checkDist);
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(speed2.normalized.x, 0, speed2.normalized.y)), out touchRay, (checkDist * 1.1f) * transform.localScale.x, 1))
        {
            //Debug.Log("Normal!");
            //Vector3 rAngle = transform.position - touchRay.point;

            Debug.Log("Hit");
            Debug.Log(touchRay.normal);
            Vector3 rAngle = touchRay.normal;
            rAngle.y = 0;
            rAngle.Normalize();
            transform.position = touchRay.point;
            transform.Translate((rAngle * 0.2f) * transform.localScale.y);
            //Debug.Log(touchRay.normal);
            CollideWallTic();
            return true;

        }
        else
        {

            return false;
        }
    }




    private void FixedUpdate() // FixedUpdate is called once per 1/60s.
    {
        MoveCharacter4Tic();
        CollideWallTic();
        TeleportXrOriginToMe();
    }

    private void TeleportXrOriginToMe()
    {
        go_XrOrigin.transform.position = transform.position;
    }

    private void JumpAbilityTic()
    {
        throw new NotImplementedException();
    }

    private void CollideFloorPitchModTic()
    {
        throw new NotImplementedException();
    }

    private void JumpHesitate()
    {
        throw new NotImplementedException();
    }

    private void JumpSwitchToFallAnimation()
    {
        throw new NotImplementedException();
    }

    private void CollideCeilingTic()
    {
        throw new NotImplementedException();
    }

    private void CollideFloorFreeFallTic()
    {
        throw new NotImplementedException();
    }

    private void MoveCharacterDuringFreeFall2Tic()
    {
        throw new NotImplementedException();
    }
}
