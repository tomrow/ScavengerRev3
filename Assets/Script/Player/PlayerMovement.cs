using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Playables;

public class PlayerMovement : MonoBehaviour
{
    //public Transform AlanAnimatorHeirarchy;
    public GameObject persistentStoragePrefab;
    public bool pitchMod;
    public bool sonic;
    public enum CameraMode { Chase, ChaseCinematic, Stay, ZipToLinear, ZipToDecelerate }
    public bool cameraLooksAtCharacter = true;
    [Tooltip("For VR Only. Set this to the axis perpendicular to the horizon.")]public Vector3 cameraLookAxes;
    public CameraMode currentCameraMode = CameraMode.Chase;
    public float camDist = 6;
    public float camHeight = 3;
    public float gravityForce = 100f;
    public bool increaseDragOnStickRelease;
    public float minSpeed, walkSpeed, runSpeed, speedCap;
    //public bool ultraMode;
    //public float turnSmoothTime = 0.2f;
    [SerializeField]float turnSmoothVelocity;
    float stickPushedFromCenter;
    public Modes playerActionMode = Modes.WalkingOrIdle;
    public enum Modes
    {
        WalkingOrIdle = 0,
        SpinAttack = 1,
        Knockback = 2,
        Punching = 3,
        Dragging = 4,
        Falling = 5,
        Death = 6,
        SecretDance = 7,
        JumpWindup = 8,
        Jumping = 9,
        ExtDisableControls = 10,
        Gunfire = 11,
        GetUpFromKnockback = 12,
        Skid = 13,
        WalkingPostSpin = 14,
        FallIntoSecretDance = 15,
        Talking = 16,
        ExtDisableControls2 = 17,
        ReloadCharacter = 0x7ffffffe,
        DeathDisableControls = 0x7fffffff
    }
    
    [SerializeField] Vector2 inputmag;
    public float gndAccelleration, airAccelleration, gndFriction;
    [Tooltip("Speed retained in the air over time as a fraction from 0 to 1")]public float airFriction;
    public Transform cameraT;
    Vector2 input2;
    public Vector2 speed2;
    RaycastHit touchRay;

    public float vspeed, hspeed, jumpForce;
    public int jumpHesitationFrames;
    float hesitationCounter;
    bool onJumpRamp = false;
    public float angleRun, avgPushAngle, pushAngle;
    public Transform debugCube;
    Transform dropShadow;
    GameObject launchSoundObj, jumpSoundObj;
    public Transform characterAnimator;
    AudioSource launchSoundControl, jumpSoundControl, stepSnd;
    MeshRenderer dropShadowGraphics;
    float normalisedSpeed;
    public Animator animatorMesh;
    float targetTravelAngleDeg;
    [SerializeField]float pushAngleDeg;
    [SerializeField] string characterName;
    public float deathTimer, deathTimerMax;
    [SerializeField]Vector2 slopeInclineDirection;
    public float speedAffectedBySlopes = 1.3f;

    public float CameraZipDurationSeconds, CameraZipPercentage;
    public Vector3 CameraZipTarget, CameraZipLookAtTarget;
    Vector3 zipStartPos;
    AnimatorStateInfo animatorStateInfo;
    public Vector3 knockBackDir;
    public bool beingAttackedByEnemy = false, attacking = false;
    float attackCoolDownLength = 1f;
    public float attackCoolDownTimer = 0;
    public float invulnSeconds;
    [SerializeField]float stickSwing;
    [SerializeField]float speedPercent;
    [SerializeField] Vector2 skidSpeed;
    public GameObject robotDeathExplosionPrefab;
    [SerializeField] bool explosionSpawned;
    public ScavengerPersistentData persistentStorage;
    float rotateToStartLerp;
    bool sndPlayable;
    // Start is called before the first frame update
    BoxCollider hitBox;
    Vector3 initialHitBoxWidth;
    void Start()
    {
        sndPlayable = true;
        if (GameObject.FindWithTag("ScavengerPersistentStorage") == null) { Debug.Log("Error, making new Persistent Storage"); persistentStorage = Instantiate(persistentStoragePrefab).GetComponent<ScavengerPersistentData>(); }
        persistentStorage = GameObject.FindWithTag("ScavengerPersistentStorage").GetComponent<ScavengerPersistentData>();
        gameObject.GetComponent<GameStateVariables>().score = persistentStorage.caps;
        gameObject.GetComponent<GameStateVariables>().boxes = persistentStorage.boxes;
        hitBox = gameObject.GetComponent<BoxCollider>();
        initialHitBoxWidth = hitBox.size;
        Debug.Log("Game Start!!");
        if (cameraT == null) { cameraT = Camera.main.transform; }
        //Get component collections
        debugCube = transform.Find("Data");
        launchSoundObj = debugCube.Find("Sound").Find("LaunchSound").gameObject;
        jumpSoundObj = debugCube.Find("Sound").Find("JumpSound").gameObject;
        launchSoundControl = launchSoundObj.GetComponent<AudioSource>();
        jumpSoundControl = jumpSoundObj.GetComponent<AudioSource>();
        dropShadow = transform.Find("dropShadow");
        dropShadowGraphics = dropShadow.gameObject.GetComponent<MeshRenderer>();
        LoadCharacter();


        //angleRun = 0;
    }
    void LoadCharacter()
    {
        stepSnd = debugCube.Find("Sound").Find("StepSoft").gameObject.GetComponent<AudioSource>();
        
        persistentStorage = GameObject.FindWithTag("ScavengerPersistentStorage").GetComponent<ScavengerPersistentData>();
        Debug.Log(persistentStorage);
        characterName = persistentStorage.currentCharacter;
        if (persistentStorage == null) { Debug.Log("Error, making new Persistent Storage"); persistentStorage = Instantiate(persistentStoragePrefab).GetComponent<ScavengerPersistentData>(); characterName = "Alan"; }

        characterAnimator = transform.Find("body");
        foreach (Transform child in characterAnimator)
        {
            if ((child.name != characterName) && (child.tag != "Orbit"))
            { child.gameObject.SetActive(false); }
            if (child.name == characterName) 
            { 
                child.gameObject.SetActive(true);
                if (characterName == "Robot")
                {
                    //stepSnd = debugCube.Find("Sound").Find("StepHard").gameObject.GetComponent<AudioSource>();
                    Transform parts = child.Find("robotAnimation");
                    Transform torsos = parts.Find("torsos");
                    foreach (Transform grandchild in parts)
                    {
                        //disable swappable parts except the ones that have been picked
                        Debug.Log(grandchild.name);
                        if (grandchild.name.StartsWith("head")){if (grandchild.name == "head" + persistentStorage.currentHead) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); }}
                        if (grandchild.name.StartsWith("torso")) { if (grandchild.name == "torso" + persistentStorage.currentTorso) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                        if (grandchild.name == "torsos") { grandchild.gameObject.SetActive(true); }
                        if (grandchild.name.StartsWith("larm")) { if (grandchild.name == "larm" + persistentStorage.currentArms) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                        if (grandchild.name.StartsWith("rarm")) { if (grandchild.name == "rarm" + persistentStorage.currentArms) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                        if (grandchild.name.StartsWith("lfist")) { if (grandchild.name == "lfist" + persistentStorage.currentArms) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                        if (grandchild.name.StartsWith("rfist")) { if (grandchild.name == "rfist" + persistentStorage.currentArms) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }

                        if (grandchild.name.StartsWith("lhand")) { grandchild.gameObject.SetActive(false); }
                        if (grandchild.name.StartsWith("rhand")) { grandchild.gameObject.SetActive(false); }

                        if (grandchild.name.StartsWith("lleg")) { if (grandchild.name == "lleg" + persistentStorage.currentLegs) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                        if (grandchild.name.StartsWith("rleg")) { if (grandchild.name == "rleg" + persistentStorage.currentLegs) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                        if (grandchild.name.StartsWith("lfoot")) { if (grandchild.name == "lfoot" + persistentStorage.currentLegs) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                        if (grandchild.name.StartsWith("rfoot")) { if (grandchild.name == "rfoot" + persistentStorage.currentLegs) { grandchild.gameObject.SetActive(true); } else { grandchild.gameObject.SetActive(false); } }
                    }
                    foreach (Transform ggrandchild in torsos)
                    {
                        //disable swappable parts except the ones that have been picked
                        Debug.Log(ggrandchild.name);
                        if (ggrandchild.name.StartsWith("torso")) { if (ggrandchild.name == "torso" + persistentStorage.currentTorso) { ggrandchild.gameObject.SetActive(true); } else { ggrandchild.gameObject.SetActive(false); } }

                    }
                }
            }
        }
        animatorMesh = GetComponentInChildren<Animator>();
    }
    #region CharacterMovement
    private void CollideWallTic()
    {

        float checkDist = speed2.magnitude * Time.fixedDeltaTime > 0.3f ? speed2.magnitude * Time.fixedDeltaTime : 0.3f;
        //checkDist = (speed2.magnitude * Time.fixedDeltaTime) * 4;
        checkDist += 0.1f;
        //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, -1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, -1) * checkDist), Color.blue);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * checkDist), Color.blue);
        //Debug.Log(checkDist);
        for (float rot = 0; rot < 2; rot += 0.125f) //increments of 0.125 degrees collision rays
        {
            Vector3 colAngle = new Vector3(Mathf.Sin(rot * Mathf.PI), 0, Mathf.Cos(rot * Mathf.PI));
            bool rc;
            if (sonic)
            { rc = (Physics.Raycast(transform.position, transform.TransformDirection(colAngle), out touchRay, (checkDist * transform.localScale.x), 1)); }
            else
            { rc = (Physics.Raycast(transform.position, colAngle, out touchRay, (checkDist * transform.localScale.x), 1)); }
            Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(colAngle), Color.red);
            Debug.DrawLine(transform.position, transform.position + colAngle, Color.green);
            Debug.DrawLine(transform.position, touchRay.point, Color.blue);
            if (rc)
            {
                //Debug.Log("Ray was cast horizontally, and we got a hit!");
                transform.position = touchRay.point;
                //Vector3 colnorm = touchRay.normal;
                //colnorm.y = 0;
                transform.Translate(colAngle * (transform.localScale.z * -0.4f), Space.World);
                if (playerActionMode == Modes.Falling || playerActionMode == Modes.Jumping)
                { transform.rotation = Quaternion.identity; }
                //speed2.x *= 0.8f;
                //speed2.y *= 0.8f;
            }
        }



    }
    private int CollideFloorPitchModTic(Modes leaveGround, Modes stayGround)
    {
        int notDead = 0;
        bool rc;
        if (sonic)
        { rc = (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up) * -1f, out touchRay, transform.localScale.y * 0.6f, 1)); }
        else
        { rc = (Physics.Raycast(transform.position, Vector3.up * -1, out touchRay, transform.localScale.y * 0.6f, 1)); }
        //Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(transform.up * -1f), Color.red);
        //Debug.DrawLine(transform.position, transform.position + (transform.up * -1f), Color.green);
        //Debug.DrawLine(transform.position, touchRay.point, Color.blue);


        if (rc)
        {
            float zAng = Mathf.Atan2(touchRay.normal.x, touchRay.normal.y) * (0 - Mathf.Rad2Deg); float xAng = Mathf.Atan2(touchRay.normal.z, touchRay.normal.y) * (Mathf.Rad2Deg);
            if (Mathf.Abs(zAng) < 0.1) { zAng = 0; }
            if (Mathf.Abs(xAng) < 0.1) { xAng = 0; }
            if (pitchMod) { transform.localEulerAngles = new Vector3(xAng, 0, zAng); }
            else { transform.localRotation = Quaternion.identity; }
            //Debug.Log("Ray was cast downward, and we got a hit!");
            //Debug.Log(transform.position.z - touchRay.point.z);

            transform.position = new Vector3
                (Mathf.Abs(touchRay.point.x - transform.position.x) < 0.002 ? transform.position.x : touchRay.point.x,
                Mathf.Abs(touchRay.point.y - transform.position.y) < 0.002 ? transform.position.y : touchRay.point.y,
                Mathf.Abs(touchRay.point.z - transform.position.z) < 0.002 ? transform.position.z : touchRay.point.z);

            slopeInclineDirection = new Vector2(touchRay.normal.x, touchRay.normal.z);
            //Debug.Log(touchRay.normal);
            Vector3 upwards = (Vector3.up * (transform.localScale.y * 0.48f));
            if (!sonic || slopeInclineDirection.magnitude < 0.1) { transform.position += upwards; }
            else{ transform.Translate(upwards); }
            //{
            //    upwards = touchRay.normal * (transform.localScale.y * 0.48f);
            //    Debug.Log(upwards);
            //    upwards = new Vector3(MathF.Round(upwards.x, 3, MidpointRounding.ToEven), MathF.Round(upwards.y, 3, MidpointRounding.ToEven), MathF.Round(upwards.z, 3, MidpointRounding.ToEven));
            //    transform.position += upwards;
            //}

            if (vspeed < 0)
            {
                vspeed = 0;
            }
            if (touchRay.collider.gameObject.tag == "Respawn")
            {
                //gameObject.GetComponent<GameStateVariables>().health = 0; //restart if touching death surface 
                playerActionMode = Modes.Death;
                return 1;
            }
            else
            { playerActionMode = stayGround; }
            movingPlatform stoodOnPlatform = touchRay.transform.gameObject.GetComponent<movingPlatform>();
            if (stoodOnPlatform != null) { transform.position += stoodOnPlatform.GetMovementOffsetPerTic(); } //Character should move along with moving platforms they stand on.
            

        }
        else
        {
            notDead = 2;
            //Debug.Log("Ray was cast downward, and we got a MISS! Switching player to freefall state");
            playerActionMode = leaveGround;
            if (onJumpRamp)
            {
                launchSoundControl.Play();
                //play ramp launch sound
            }

        }
        return notDead;

    }

    private int CollideFloorSpinAttackTic()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), out touchRay, transform.localScale.y * 0.6f, 1))
        {
            //Debug.Log("Ray was cast downward, and we got a hit!");
            transform.position = touchRay.point;
            slopeInclineDirection = new Vector2(touchRay.normal.x, touchRay.normal.z);
            transform.Translate(transform.up * (transform.localScale.y * 0.48f), Space.World);
            if (vspeed < 0)
            {
                vspeed = 0;
            }
            if (touchRay.collider.gameObject.tag == "Respawn")
            {
                //gameObject.GetComponent<GameStateVariables>().health = 0; //restart if touching death surface
                playerActionMode = Modes.Death;
                return 1;
            }
            movingPlatform stoodOnPlatform = touchRay.transform.gameObject.GetComponent<movingPlatform>();
            if (stoodOnPlatform != null) { transform.position += stoodOnPlatform.GetMovementOffsetPerTic(); } //Character should move along with moving platforms they stand on.


        }
        else
        {
            //Debug.Log("Ray was cast downward, and we got a MISS! Switching player to freefall state");
            //playerActionMode = Modes.Falling;
            if (onJumpRamp)
            {
                launchSoundControl.Play();
                //play ramp launch sound
            }

        }
        return 0;

    }
    private void CollideFloorFreeFallTic(bool holdingJump, Modes reachGround)
    {
        float vray = vspeed < 0 ?  0-vspeed : 0;
        vray *= Time.fixedDeltaTime;
        //if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), out touchRay, (transform.localScale.y * (0.5f + vray)), 1))
        if (Physics.Raycast(transform.position, (new Vector3(0, -1, 0)), out touchRay, (transform.localScale.y * (0.5f + vray)), 1))
        {
            //Debug.Log("Ray was cast downward, and we got a hit! Switching back to running mode");
            playerActionMode = reachGround;
            stepSnd.Play();
            if (input2.magnitude < 0.15f) { speed2 *= 0f; }
            transform.position = touchRay.point;
            transform.Translate(transform.up * (transform.localScale.y * 0.5f), Space.World);
            if (vspeed < 0)
            {
                vspeed = 0;
            }
            //TODO: Check for death surfaces.

        }
        else
        {
            vspeed -= gravityForce * Time.fixedDeltaTime; //gravity
            if(!holdingJump && vspeed>0) { vspeed -= gravityForce * Time.fixedDeltaTime; } //lower jump when not holding key (hack)
        }
        rotateToStartLerp += Time.fixedDeltaTime/8f;
        if (rotateToStartLerp > jumpForce/8) { rotateToStartLerp = 1; }
        if (vspeed>0) { rotateToStartLerp = 0; }
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, rotateToStartLerp);
        if (vspeed > 0) { transform.Translate((new Vector3(0, vspeed, 0) * transform.localScale.y) * Time.fixedDeltaTime, Space.Self); }
        else { transform.Translate((new Vector3(0, vspeed, 0) * transform.localScale.y) * Time.fixedDeltaTime, Space.World); }
        //hspeed = 0f;

    }

    private void MoveCharacterDuringFreeFall2Tic()
    {
        input2 = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); //Stick position
        Vector2 preinputmag = input2 * runSpeed; //Multiply to reach intended speed values for later math
        float pushAngle = (Mathf.Atan2(input2.x, input2.y)) + (cameraT.eulerAngles.y * Mathf.Deg2Rad) % 360; //Get stick angle and add camera angle to it, so that forwards is always forwards relative to camera
        targetTravelAngleDeg = pushAngle * Mathf.Rad2Deg;
        pushAngleDeg = pushAngle * Mathf.Rad2Deg;
        Vector2 inputmag = new Vector2(Mathf.Sin(pushAngle), Mathf.Cos(pushAngle)); //prevent weird rotational stumblings re. MoveCharacter4Tic
        inputmag = inputmag.normalized * (preinputmag.magnitude);
        inputmag.x = (float)(Mathf.Round(inputmag.x * 10000f) / 10000);
        inputmag.y = (float)(Mathf.Round(inputmag.y * 10000f) / 10000); //4 decimal places

        if (Vector2.Angle(speed2, inputmag) > 100)//if the stick is being held the opposite direction to the movement direction, brake
        { speed2 *= airFriction; } //Debug.Log("Air Braking"); }

        Vector2 forceAdd = inputmag.normalized * (hspeed * 1.4f);
        
        speed2.x += ((inputmag.x - speed2.x) * airAccelleration) * Time.fixedDeltaTime; //add player input movement
        speed2.y += ((inputmag.y - speed2.y) * airAccelleration) * Time.fixedDeltaTime;
        speed2.x += forceAdd.x;                                                         //add extra speed from ramps
        speed2.y += forceAdd.y;
        //Debug.Log(Vector2.Angle(speed2, inputmag));
        
        if (input2.magnitude < 0.1) { speed2 *= 0.99f; }

        hspeed = 0;
        if (Math.Abs(speed2.x) < minSpeed)
        {
            speed2.x = 0;
        }
        if (Math.Abs(speed2.y) < minSpeed) //prevent floating point shenanigans
        {
            speed2.y = 0;
        }

        stickPushedFromCenter = speed2.magnitude / (runSpeed / 4);
        if (input2.magnitude > 0.1f)
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
        normalisedSpeed = speed2.magnitude;
        speed2 = speed2.normalized * (normalisedSpeed > speedCap ? speedCap : normalisedSpeed);
        transform.Translate((new Vector3(speed2.x, 0, speed2.y) * transform.localScale.x) * Time.fixedDeltaTime, Space.Self);
        //characterAnimator.eulerAngles = new Vector3(characterAnimator.eulerAngles.x, angleRun, characterAnimator.eulerAngles.z);
    }
    private void MoveCharacter5Tic()
    {
        
        input2 = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector2 preinputmag = input2 * runSpeed; //Multiply to reach intended speed values for later math


        float targetTravelAngle = (Mathf.Atan2(input2.x, input2.y)) + (cameraT.eulerAngles.y * Mathf.Deg2Rad); //Get stick angle and add camera angle to it, so that forwards is always forwards relative to camera
        targetTravelAngleDeg = targetTravelAngle * Mathf.Rad2Deg;
        targetTravelAngleDeg = targetTravelAngleDeg % 360;  ///////////////////////// Keep angles within range
        while (targetTravelAngleDeg > 180) { targetTravelAngleDeg -= 360; }
        while (targetTravelAngleDeg < -180) { targetTravelAngleDeg += 360; }
        //Debug.Log(targetTravelAngleDeg);
        if (input2.magnitude > 0.02)
        {
            if (speed2.magnitude > minSpeed * 300)
            {
                pushAngleDeg = Mathf.SmoothDampAngle(pushAngleDeg, targetTravelAngleDeg, ref turnSmoothVelocity, Mathf.Clamp(speedPercent * 0.85f,0.05f,1f), Mathf.Infinity, Time.fixedDeltaTime); //Smoothly rotate and limit cornering speed
                pushAngleDeg = pushAngleDeg % 360;
                while(pushAngleDeg > 180)  { pushAngleDeg -= 360; }
                while(pushAngleDeg < -180) { pushAngleDeg += 360; }
                stickSwing = Mathf.Abs(pushAngleDeg - targetTravelAngleDeg);
                if(stickSwing > 180) 
                {
                    stickSwing = 360 - stickSwing;
                    //Debug.Log("StickSwing >180!");
                    //Debug.Log(stickSwing);
                }
            }
            else
            {
                stickSwing = 0;
                pushAngleDeg = targetTravelAngleDeg;
            }
        }
        pushAngle = pushAngleDeg * Mathf.Deg2Rad;  //c# radian nonsense, why cant I set it to degrees for everything?
        inputmag = new Vector2(Mathf.Sin(targetTravelAngle), Mathf.Cos(targetTravelAngle));
        inputmag = inputmag.normalized * (preinputmag.magnitude > speedCap ? speedCap : preinputmag.magnitude); //recalculate stick values based on calculated angle
        inputmag.x = MathF.Round(inputmag.x, 4, MidpointRounding.ToEven);
        inputmag.y = MathF.Round(inputmag.y, 4, MidpointRounding.ToEven); //4

        /*if (Math.Abs(avgPushAngle - pushAngle) > Mathf.PI * 0.6f)
        {
            speed2 = speed2 * 0.7f; //speed braking thing that I'm not sure is doing anything but it doesnt seem to be causing any weirdness so leave it in or delete it if you want
        }*/
        avgPushAngle = (avgPushAngle + pushAngle) / 2; //mean current angle with the one from last tic

        Vector2 forceAdd = inputmag.normalized * (hspeed * 4);
        if (speed2.magnitude < inputmag.magnitude)
        {
            speed2.x += ((inputmag.x - speed2.x) * gndAccelleration) * Time.fixedDeltaTime;
            speed2.y += ((inputmag.y - speed2.y) * gndAccelleration) * Time.fixedDeltaTime;
        }
        else if (speed2.magnitude > inputmag.magnitude)
        {
            speed2 *= Mathf.Clamp01( gndFriction - Time.fixedDeltaTime);
        }
        speed2 = new Vector2(Mathf.Sin(pushAngle), Mathf.Cos(pushAngle)) * speed2.magnitude;

        speed2.x += forceAdd.x;
        speed2.y += forceAdd.y; //TODO: for ramps and speed pads, this needs work
        normalisedSpeed = speed2.magnitude;
        speed2 = speed2.normalized * (normalisedSpeed > speedCap ? speedCap : normalisedSpeed); //speed limit
        slopeInclineDirection = slopeInclineDirection.normalized * (slopeInclineDirection.magnitude > 0.1f ? slopeInclineDirection.magnitude : 0f);
        speed2 += (slopeInclineDirection * (speedAffectedBySlopes)) * Time.fixedDeltaTime; //struggle to go up slopes, and run fast down them
        //speed2 *= (stickSwing / 180);
        hspeed = 0;
        if (slopeInclineDirection.magnitude < 0.2f)
        {
            if (Math.Abs(speed2.x) < minSpeed)
            {
                speed2.x = 0;
            }
            if (Math.Abs(speed2.y) < minSpeed)
            {
                speed2.y = 0;
            } //eliminate floating point shenanigans so that zero means zero
        }
        //speed2 += (slopeInclineDirection.normalized * (speedAffectedBySlopes * (slopeInclineDirection.magnitude*slopeInclineDirection.magnitude) )) * Time.fixedDeltaTime; //slide down slopes if not pushing the stick
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
        else
        {
            if (increaseDragOnStickRelease)
            {
                speed2 *= Mathf.Clamp01(0.99f-Time.fixedDeltaTime);
            }
        }
        if (stickSwing > 80 && speed2.magnitude > 3) { skidSpeed = speed2 * 0.8f; playerActionMode = Modes.Skid; }
        else
        {speed2 *= 1 - Mathf.Clamp01((stickSwing / 180) * 1.1f) * (Time.fixedDeltaTime * 2);} //Brake if the swing is large (pushing stick the other way
        ////////////////////////////////////////////////////////////////////////////////multiply fixedDeltaTime for stronger braking
        if (!WallColMoveTic()) { transform.Translate((new Vector3(speed2.x, 0, speed2.y) * transform.localScale.x) * Time.fixedDeltaTime, Space.Self); }
        characterAnimator.localEulerAngles = new Vector3(characterAnimator.localEulerAngles.x, angleRun, characterAnimator.localEulerAngles.z);
        speedPercent = speed2.magnitude / speedCap;
    }
    private void JumpAbilityTic()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), out touchRay, (transform.localScale.y * 0.5f), 1))
        {
            if (touchRay.collider.gameObject.tag == "JumpStorageBoost")
            {
                vspeed = 40;
                hspeed = 150;
                onJumpRamp = true;
            }
            else
            {
                if (vspeed < 0)
                {
                    vspeed = 0;


                }
                hspeed = 0;
                onJumpRamp = false;
            }
        }
        if (Input.GetAxis("Fire1") == 1f)
        {
            vspeed = jumpForce; // 70f;
            hspeed = 0f;

            //transform.Translate(transform.up * 2 , Space.World);
            //this is no longer needed here as jumping has been moved
            playerActionMode = Modes.JumpWindup;
            hesitationCounter = 0;
        }
    }
    private void JumpHesitate()
    {
        if (hesitationCounter < (jumpHesitationFrames) * (1 / 60))
        {
            hesitationCounter += Time.fixedDeltaTime;
        }
        else
        {
            transform.Translate(transform.up * (transform.localScale.y * 0.25f), Space.World);
            playerActionMode = Modes.Jumping;
            jumpSoundControl.Play();
        }
    }
    private void JumpSwitchToFallAnimation()
    {
        if (vspeed < 0f)
        {
            playerActionMode = Modes.Falling;
        }
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
        //Debug.Log(checkDist);
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(speed2.normalized.x, 0, speed2.normalized.y)), out touchRay, (checkDist * 1.1f) * transform.localScale.x, 1))
        {
            //Debug.Log("Normal!");
            //Vector3 rAngle = transform.position - touchRay.point;
            //Debug.Log("Hit");
            //Debug.Log(touchRay.normal);
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
    private int CollideCeilingTic()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 1, 0) * 0.6f), Color.green);
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, 1, 0)), out touchRay, (transform.localScale.y * 0.6f) + (vspeed * Time.fixedDeltaTime *4), 1))
        {
            transform.rotation = Quaternion.identity;
            //Debug.Log("Ray was cast upward, and we got a hit!");
            bool playerAboveHitCeiling = transform.position.y > touchRay.point.y;
            transform.position = touchRay.point;

            transform.Translate(transform.up * (transform.localScale.y * (playerAboveHitCeiling ? 0.61f : -0.61f)), Space.World);
            transform.rotation = Quaternion.identity;
            vspeed *= -0.1f;
            //transform.Translate(transform.up * (transform.localScale.y * (0-Mathf.Abs(vspeed)) ), Space.World);


            if (touchRay.collider.gameObject.tag == "Respawn")
            {
                //gameObject.GetComponent<GameStateVariables>().health = 0; //restart if touching death surface
                playerActionMode = Modes.Death;
                return 1;
            }



        }
        return 0;
    }
    private void MoveCharacterSkidTic()
    {
        speed2 = Vector2.zero;
        //skidSpeed = skidSpeed.normalized * (skidSpeed.magnitude * (Mathf.Clamp01(gndFriction*0.95f)*0.99f));
        skidSpeed *= Mathf.Clamp01((gndFriction * 0.9f)-Time.fixedDeltaTime);
        if (!WallColMoveTic()) { transform.Translate((new Vector3(skidSpeed.x, 0, skidSpeed.y) * transform.localScale.x) * Time.fixedDeltaTime, Space.World); }
        if(skidSpeed.magnitude < gndFriction / 2) { playerActionMode = Modes.WalkingOrIdle; }
    }
    #endregion

    #region CameraControlsAndBehaviour
    private void OrbitCamera(Vector2 camOfs)
    {
        Vector2 rStick = new Vector2(Input.GetAxis("RightStickHorizontal"), Input.GetAxis("RightStickVertical"));
        camDist -= (rStick.y * 4) * Time.fixedDeltaTime;
        //cameraT.position = transform.position + new Vector3(camOfs.x, 0, camOfs.y).normalized * (camDist*Time.fixedDeltaTime);
        cameraT.Translate(rStick.x * Time.fixedDeltaTime * 8, 0, (rStick.y * Time.fixedDeltaTime * 4));
    }
    private void SpinAttack()
    {
        if (attackCoolDownTimer >= attackCoolDownLength && Input.GetAxis("Fire2") > 0.5f)
        {
            attackCoolDownTimer = 0;
            playerActionMode = Modes.SpinAttack;
        }
    }
    #endregion
    private void FixedUpdate() // FixedUpdate is called once per 1/60s.
    {
        
        attackCoolDownTimer += Time.fixedDeltaTime;
        attackCoolDownTimer = Mathf.Clamp(attackCoolDownTimer, 0, attackCoolDownLength);
        Vector2 characterHorizontalPosition = new Vector2(transform.position.x, transform.position.z); ;
        Vector2 horizontalCameraOffset = new Vector2(cameraT.position.x, cameraT.position.z) - characterHorizontalPosition;
        if (cameraLooksAtCharacter)
        { cameraT.LookAt(
            transform.position +
            transform.TransformDirection(new Vector3(
                Mathf.Clamp(speed2.x / 16, -4, 4),
                Mathf.Clamp(vspeed/32,-1,0),
                Mathf.Clamp(speed2.y / 16, -4, 4) )) + Vector3.up*0.5f);
            cameraT.localEulerAngles = new Vector3(
                MathF.Round(cameraT.localEulerAngles.x/200, 4, MidpointRounding.ToEven) * 200 * cameraLookAxes.x, 
                MathF.Round(cameraT.localEulerAngles.y/200, 4, MidpointRounding.ToEven) * 200 * cameraLookAxes.y, 
                MathF.Round(cameraT.localEulerAngles.z/200, 4, MidpointRounding.ToEven) * 200 * cameraLookAxes.z);
            //cameraT.position = new Vector3(
            //    MathF.Round(cameraT.position.x / 200, 3, MidpointRounding.ToEven) * 200,
            //    MathF.Round(cameraT.position.y / 200, 3, MidpointRounding.ToEven) * 200,
            //    MathF.Round(cameraT.position.z / 200, 3, MidpointRounding.ToEven) * 200);

        }
        switch (currentCameraMode)
        {
            case CameraMode.Chase:
                characterHorizontalPosition = new Vector2(transform.position.x, transform.position.z); ;
                horizontalCameraOffset = new Vector2(cameraT.position.x, cameraT.position.z) - characterHorizontalPosition;
                //horizontalCameraOffset = horizontalCameraOffset.normalized * (0- camDist);
                //Debug.Log(horizontalCameraOffset.magnitude);
                if (horizontalCameraOffset.magnitude > camDist)
                {
                    horizontalCameraOffset = horizontalCameraOffset.normalized * (camDist);
                    horizontalCameraOffset += characterHorizontalPosition;
                    cameraT.position = new Vector3(horizontalCameraOffset.x, transform.position.y + camHeight, horizontalCameraOffset.y);
                }
                else
                { cameraT.position = new Vector3(cameraT.position.x, transform.position.y + camHeight, cameraT.position.z); }
                CameraZipPercentage = 0f;
                Vector3 dist = cameraT.position - transform.position;
                if (cameraLooksAtCharacter)
                { OrbitCamera(horizontalCameraOffset); }
                if (dist.magnitude < camDist * 1.5f)
                {
                    dist = cameraT.position - transform.position;
                    cameraT.Translate(Vector3.forward * (0 - Mathf.Abs(dist.magnitude - camDist)) );
                    Debug.Log("Camera is too close to character");
                    Debug.Log(dist.magnitude);
                }
                break;
            case CameraMode.ChaseCinematic:
                characterHorizontalPosition = new Vector2(transform.position.x, transform.position.z);
                horizontalCameraOffset = new Vector2(cameraT.position.x, cameraT.position.z) - characterHorizontalPosition;
                //horizontalCameraOffset = horizontalCameraOffset.normalized * (0- camDist);
                //Debug.Log(horizontalCameraOffset.magnitude);
                if (horizontalCameraOffset.magnitude > camDist)
                {
                    horizontalCameraOffset = horizontalCameraOffset.normalized * (camDist);
                    horizontalCameraOffset += characterHorizontalPosition;
                    cameraT.position = new Vector3(horizontalCameraOffset.x, transform.position.y + camHeight, horizontalCameraOffset.y);
                }
                //else
                //{ cameraT.position = new Vector3(cameraT.position.x, transform.position.y + camHeight, cameraT.position.z); }
                CameraZipPercentage = 0f;
                if (cameraLooksAtCharacter)
                { OrbitCamera(horizontalCameraOffset); }
                break;
            case CameraMode.Stay:
                CameraZipPercentage = 0f;
                break;
            case CameraMode.ZipToDecelerate:
                if (!cameraLooksAtCharacter) { cameraT.LookAt(CameraZipLookAtTarget); }
                if (CameraZipPercentage <= 0) { zipStartPos = cameraT.position; }
                CameraZipPercentage += (Time.fixedDeltaTime / CameraZipDurationSeconds);
                CameraZipPercentage = Mathf.Clamp01(CameraZipPercentage);
                cameraT.position = Vector3.Lerp(zipStartPos, CameraZipTarget, (CameraZipPercentage));
                if (CameraZipPercentage >= 1) { currentCameraMode = CameraMode.Stay; }
                break;
            case CameraMode.ZipToLinear:
                if (!cameraLooksAtCharacter) { cameraT.LookAt(CameraZipLookAtTarget); }
                if (CameraZipPercentage <= 0) { zipStartPos = cameraT.position; }
                CameraZipPercentage += (Time.fixedDeltaTime / CameraZipDurationSeconds);
                CameraZipPercentage = Mathf.Clamp01(CameraZipPercentage);
                cameraT.position = Vector3.Lerp( zipStartPos, CameraZipTarget, 0.5f*( 1-Mathf.Cos(Mathf.PI*CameraZipPercentage) ) );
                if(CameraZipPercentage>=1) { currentCameraMode = CameraMode.Stay; }
                break;
            default:
                throw new NotImplementedException();
        }

        animatorMesh.SetInteger("mode", (int)playerActionMode);
        animatorStateInfo = animatorMesh.GetCurrentAnimatorStateInfo(0); //get animation timer for animation dependent modes (knockback only ends once alan hits the floor, and only then can he get up)
        invulnSeconds -= Time.fixedDeltaTime;
        if (invulnSeconds < 0) {  invulnSeconds = 0; }
        if (invulnSeconds<=0 && beingAttackedByEnemy)
        {
            playerActionMode = Modes.Knockback;
            ////////////////////////////////////////////TODO: reduce health
        }
        switch (playerActionMode)
        {
            case Modes.WalkingOrIdle:
                //running
                animatorMesh.SetFloat("speed", speed2.magnitude / (speedCap / 12));
                //Debug.Log(speed2.magnitude / (speedCap / 12));
                MoveCharacter5Tic();
                CollideWallTic();
                JumpAbilityTic();
                CollideFloorPitchModTic(Modes.Falling, playerActionMode);
                SpinAttack();
                playStepSoundsWhileWalking();
                break;
            case Modes.Skid:
                //running
                MoveCharacterSkidTic();
                CollideWallTic();
                JumpAbilityTic();
                CollideFloorPitchModTic(Modes.Falling, playerActionMode);
                SpinAttack();
                break;
            case Modes.Knockback:
                //stuck
                CollideWallTic();
                CollideFloorPitchModTic(Modes.Falling, playerActionMode);
                if (animatorStateInfo.IsName("knockBack") && animatorStateInfo.normalizedTime > 0.6f)
                { playerActionMode = Modes.GetUpFromKnockback; }
                else
                { 
                    transform.position += (knockBackDir.normalized * 6) * Time.fixedDeltaTime; //go backwards when hit by enemy
                    invulnSeconds = 7;
                    speed2 = Vector2.zero;
                }
                break;
            case Modes.Falling:
                //falling
                MoveCharacterDuringFreeFall2Tic();
                CollideFloorFreeFallTic(false, Modes.WalkingOrIdle);
                CollideWallTic();
                CollideCeilingTic();
                SpinAttack();
                break;
            case Modes.Death:
                Debug.Log("Death");
                Debug.Log(playerActionMode);
                //death
                deathTimer += Time.fixedDeltaTime;
                if (characterName == "Robot" && animatorStateInfo.IsName("knockBack") && animatorStateInfo.normalizedTime > 0.5f && !explosionSpawned)
                { Instantiate(robotDeathExplosionPrefab, transform.position, Quaternion.identity); Debug.Log("Boom"); explosionSpawned = true; characterAnimator.gameObject.SetActive(false); }
                if (deathTimer > deathTimerMax)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    playerActionMode = Modes.DeathDisableControls; //way out of bounds so nothing will happen
                    //well, not any more since switchcase change since it'll throw an exception on unimplemented mode numbers, but i'm keeping this number anyway
                }
                break;
            case Modes.DeathDisableControls:
                //disable controls after death
                break;
            case Modes.JumpWindup: 
                MoveCharacter5Tic();
                CollideWallTic();
                CollideFloorPitchModTic(Modes.Falling, playerActionMode);
                SpinAttack();
                JumpHesitate(); //jump windup
                break;
            case Modes.Jumping:
                //jumping upward animation hack
                MoveCharacterDuringFreeFall2Tic();
                CollideFloorFreeFallTic(Input.GetAxis("Fire1") > 0.8f, Modes.WalkingOrIdle);
                SpinAttack();
                CollideWallTic();
                CollideCeilingTic();
                JumpSwitchToFallAnimation();
                break;
            case Modes.ExtDisableControls:
                //disable controls for zipline
                break;
            case Modes.ExtDisableControls2:
                //disable controls for zipline
                break;
            case Modes.GetUpFromKnockback:
                //stuck
                CollideWallTic();
                if (animatorStateInfo.IsName("knockBackGetUp") && animatorStateInfo.normalizedTime > 0.9f)
                { playerActionMode = Modes.WalkingOrIdle; }
                else
                { invulnSeconds = 7; }
                break;
            case Modes.SpinAttack:
                hitBox.size = initialHitBoxWidth + new Vector3(1.2f,0,1.2f);
                MoveCharacterDuringFreeFall2Tic();
                CollideWallTic();
                CollideCeilingTic();
                CollideFloorSpinAttackTic();
                if (animatorStateInfo.IsName("spin") && animatorStateInfo.normalizedTime > 0.9f)
                { playerActionMode = Modes.Falling; vspeed = 0f; hitBox.size = initialHitBoxWidth; }
                break;
            case Modes.WalkingPostSpin: //this mode is just the walking mode but without the jump cmd, so that exiting from a midair spin doesnt allow you to jump in midair on the first frame
                //running
                animatorMesh.SetFloat("speed", speed2.magnitude / (speedCap / 12));
                //Debug.Log(speed2.magnitude / (speedCap / 12));
                playerActionMode = Modes.WalkingOrIdle;
                MoveCharacter5Tic();
                CollideWallTic();
                //JumpAbilityTic();
                CollideFloorPitchModTic(Modes.Falling, playerActionMode);
                SpinAttack();
                break;
            case Modes.SecretDance:
                invulnSeconds = 3;
                characterAnimator.localEulerAngles = Vector3.up * (cameraT.transform.localEulerAngles.y + 180);
                speed2 = Vector2.zero;
                if (animatorStateInfo.IsName("success") && animatorStateInfo.normalizedTime > 0.99f)
                {
                    //Animation is over or not started, get back to game
                    playerActionMode = Modes.WalkingOrIdle;
                    
                }
                break;
            case Modes.FallIntoSecretDance:
                CollideFloorFreeFallTic(false, Modes.SecretDance);
                break;
            case Modes.Talking:
                //UI interaction code goes here
                break;
            case Modes.ReloadCharacter:
                LoadCharacter();
                playerActionMode = Modes.WalkingOrIdle;
                break;

            default: throw new NotImplementedException();
        }
        attacking = playerActionMode == Modes.SpinAttack;
        beingAttackedByEnemy = false; //Reset this so the player isnt being knocked back for eternity
        
        //drop shadow
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), out touchRay, 9999, 1))
        {
            //enable plane renderer and move shadow transform to raycast hit point
            dropShadowGraphics.enabled = true;
            dropShadow.position = touchRay.point + ((Vector3.up * 0.02f) * transform.localScale.y);
        }
        else
        {
            //missed, maybe above a bottomless pit? whatever the ground is so far below we can hide the drop shadow
            dropShadowGraphics.enabled = false;
        }
        if (Input.GetButton("Cancel"))
        {
            SceneManager.LoadScene("ScavengerTitleScreen");
        }

    }

    private void playStepSoundsWhileWalking()
    {
        float every = 0.5f;
        float range = 0.1f;
        float offset = 0;
        if (persistentStorage.currentCharacter == "Alan") 
        {
            if (animatorStateInfo.IsName("walk")) { offset = 0f; }
            else if (animatorStateInfo.IsName("run")) { offset = 0.25f; }
            else if (animatorStateInfo.IsName("sprint")) { offset = 0.28f; }
            else { sndPlayable = false; }
            //walk  every = 0.5f offset=0f
            //run   every = 0.5f offset=-0.25f
            //sprint   every = 0.5f offset=-0.28f
        }
        if (persistentStorage.currentCharacter == "Robot")
        {
            if (animatorStateInfo.IsName("walk")) { offset = 0f; }
            else if (animatorStateInfo.IsName("run")) { offset = 0f; }
            else if (animatorStateInfo.IsName("sprint")) { offset = 0.25f; }
            else { sndPlayable = false; }
            //walk  every = 0.5f offset=0f
            //run   every = 0.5f offset=-0.0f
            //sprint   every = 0.5f offset=-0.25f
        }
        if ((animatorStateInfo.normalizedTime + offset) % every < range)
        { 
            if (sndPlayable && !animatorStateInfo.IsName("fall") && !animatorStateInfo.IsName("jump"))
            { stepSnd.Play(); sndPlayable = false; }
        }
        else
        { sndPlayable = true; }
    }
}
