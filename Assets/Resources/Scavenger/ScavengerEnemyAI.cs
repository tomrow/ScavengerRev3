using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class ScavengerEnemyAI : MonoBehaviour
{
    AnimatorStateInfo animatorStateInfo;
    public float vspeed;
    public bool backAndForthPacing = true;
    public bool flying = false;
    public bool attackAnim = true;
    public bool chase = false;
    public float chaseSpeedMetersPerSecond = 4;
    GameObject player;
    Quaternion directionOfPlayer;
    Quaternion oldRotation;
    float gravityForce;
    int lerpSubMode = 0;
    float lerpTimer = 0;
    float waitTimer = 0;
    Vector3 lookPos = Vector3.zero;
    Vector3 movementOffsetPerTic;
    public float travelTime = 3;
    public float waitPeriod = 0.25f;
    [Tooltip("Position 1 for pacing back and forth")] public Vector3 position1;
    [Tooltip("Position 2 for pacing back and forth")] public Vector3 position2;
    Vector3 nextStart;
    public GameObject deathParticleEffectPrefab;
    public GameObject ricochetSoundEffectPrefab;
    public GameObject dropOnDeathPrefab;
    public enum Modes
    {
        WalkingOrIdle = 0,
        Swipe = 1, //for enemy lunges or alert hops
        Knockback = 2, //for enemy death
        Chase = 3, //for pursuit or gunfire
        Falling=4,
        Dead = 5
    }
    Modes actionMode = Modes.WalkingOrIdle;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount != 1)
        { Destroy(this.gameObject); }
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        { Destroy(this.gameObject); }
        player = GameObject.FindWithTag("Player");
        gravityForce = 6;
        nextStart = position1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (actionMode != Modes.Swipe) { oldRotation = transform.rotation; }
        directionOfPlayer = Quaternion.LookRotation((player.transform.position - transform.position), Vector3.up); 
        animator.SetInteger("mode", (int)actionMode);
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        switch (actionMode) 
        {
            case Modes.WalkingOrIdle:
                if (backAndForthPacing){ PaceBackAndForth(); }
                if (attackAnim) { checkPlayerProximityAndSwipe(2); }
                if (!flying) { CollideFloorTic(); }
                break;
            case Modes.Swipe:
                if (!chase)
                {
                    if (backAndForthPacing) { PaceBackAndForth(); PaceBackAndForth();} //multiple calls to multiply speed
                    if (!flying) { CollideFloorTic(); }
                    if (animatorStateInfo.IsName("swipe") && animatorStateInfo.normalizedTime > 0.9f)
                    { actionMode = Modes.WalkingOrIdle; }
                }
                else {
                    if (animatorStateInfo.IsName("swipe"))
                    { transform.rotation = Quaternion.Slerp(oldRotation, directionOfPlayer, animatorStateInfo.normalizedTime); }
                    if (animatorStateInfo.IsName("swipe") && animatorStateInfo.normalizedTime > 0.9f)
                    { actionMode = Modes.Chase; }
                }
                break;
            case Modes.Chase:
                transform.LookAt(player.transform.position);
                if (!flying) { transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); CollideFloorTic(); }
                if (WallColMoveTic()) { transform.Translate(Vector3.forward * (chaseSpeedMetersPerSecond * Time.fixedDeltaTime)); }
                CollideWallTic();
                break;
            case Modes.Falling:
                CollideFloorFreeFallTic(false);
                if (WallColMoveTic()) { transform.position += movementOffsetPerTic; }
                CollideWallTic();
                break;
            case Modes.Knockback:
                CollideFloorDieTic(false);
                if (!WallColMoveTic()) { transform.position += movementOffsetPerTic * Time.fixedDeltaTime; }
                CollideWallTicDie();
                break;
            default: 
                Instantiate(deathParticleEffectPrefab, transform.position, Quaternion.identity);
                if (dropOnDeathPrefab) { Instantiate(dropOnDeathPrefab, transform.position + (Vector3.up * 0.5f), Quaternion.identity); }
                Destroy(this.gameObject);
                break;
        }
    }

    private void checkPlayerProximityAndSwipe(float v)
    {
        if (Vector3.Distance(transform.position, player.transform.position) < v)
        {
            actionMode = Modes.Swipe;
        }
    }

    private void PaceBackAndForth()
    {
        if (true)
        {
            if (lerpSubMode == 0) //travel from position 1 to position 2
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer); //mustn't leave these bounds or the player may slide around further than the platform travels
                waitTimer = 0; //set this to zero to prepare for next stage;
                Vector3 newPos = Vector3.Lerp(nextStart, position2, lerpTimer);
                lookPos = position2;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos); movementOffsetPerTic = newPos - transform.position;
                if (WallColMoveTic()) { lerpTimer = 1; }
                transform.position = new Vector3(newPos.x, flying ? newPos.y : transform.position.y, newPos.z);
                lerpSubMode += lerpTimer >= 1 ? 1 : 0; //when the lerpTimer is 1, move on to the next stage
            }
            if (lerpSubMode == 1) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero;
                transform.Rotate(0, ((Time.fixedDeltaTime/waitPeriod)*180), 0);
                lerpSubMode += waitTimer >= waitPeriod ? 1 : 0;
                nextStart = transform.position;
            }
            if (lerpSubMode == 2) //travel from position 2 to position 1
            {
                lerpTimer += Time.fixedDeltaTime / travelTime;
                lerpTimer = Mathf.Clamp01(lerpTimer); //mustn't leave these bounds or the player may slide around further than the platform travels
                waitTimer = 0; //set this to zero to prepare for next stage;
                Vector3 newPos = Vector3.Lerp(nextStart, position1, lerpTimer);
                lookPos = position1;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos); movementOffsetPerTic = newPos - transform.position;
                if (WallColMoveTic()) { lerpTimer = 1; }
                transform.position = new Vector3(newPos.x, flying ? newPos.y : transform.position.y, newPos.z);
                lerpSubMode += lerpTimer >= 1 ? 1 : 0; //when the lerpTimer is 1, move on to the next stage
            }
            if (lerpSubMode == 3) //wait for waitPeriod seconds
            {
                lerpTimer = 0;
                waitTimer += Time.fixedDeltaTime;
                movementOffsetPerTic = Vector3.zero;
                transform.Rotate(0, ((Time.fixedDeltaTime / waitPeriod) * 180), 0);
                if (waitTimer >= waitPeriod) { lerpSubMode = 0; }
                nextStart = transform.position;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy");
        if (other.gameObject.tag == "Player" && actionMode != Modes.Knockback)
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement.attacking)
            {
                actionMode = Modes.Knockback;
                Instantiate(ricochetSoundEffectPrefab, transform.position, Quaternion.identity);
                vspeed = 3;
                movementOffsetPerTic = other.transform.position - transform.position;
                //movementOffsetPerTic.Normalize();
                movementOffsetPerTic *= -10;
                transform.LookAt(other.transform.position);
            }
            else if (playerMovement.playerActionMode == PlayerMovement.Modes.Falling && playerMovement.vspeed < 0)
            {
                actionMode = Modes.Dead;
                playerMovement.vspeed = 16;
                playerMovement.playerActionMode = PlayerMovement.Modes.Jumping;
            }
            else if (playerMovement.invulnSeconds <= 0)
            {
                playerMovement.beingAttackedByEnemy = true;
                playerMovement.knockBackDir = (other.gameObject.transform.position - transform.position);
                playerMovement.knockBackDir.y = 0;
                playerMovement.knockBackDir = playerMovement.knockBackDir.normalized;
                other.gameObject.transform.Find("Data").Find("Sound").Find("Ouch").gameObject.GetComponent<AudioSource>().Play();
                other.gameObject.GetComponent<GameStateVariables>().health -= (int)(other.gameObject.GetComponent<GameStateVariables>().maxHealth / 4f);//have to have a float somewhere in this otherwise it will try integer division and get wrong answer

            }
        }
    }
    RaycastHit touchRay;
    private bool WallColMoveTic()
    {
        float checkDist = 0.5f;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * (checkDist * transform.localScale.x), Color.red);
        //Debug.Log(checkDist);
        CollideWallTic();
        //Debug.Log(transform.TransformDirection(new Vector3(speed2.normalized.x, 0, speed2.normalized.y)));
        //Debug.Log(checkDist);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out touchRay, (checkDist * 1.1f) * transform.localScale.x, 1))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * (checkDist * transform.localScale.x), Color.green);
            //Debug.Log("Normal!");
            //Vector3 rAngle = transform.position - touchRay.point;

            Debug.Log("Hit");
            Debug.Log(touchRay.normal);
            Vector3 rAngle = touchRay.normal;
            rAngle.y = 0;
            rAngle.Normalize();
            transform.position = touchRay.point;
            transform.Translate(Vector3.forward * -0.5f);
            //Debug.Log(touchRay.normal);
            CollideWallTic();
            return true;

        }
        else
        {

            return false;
        }
    }
    private void CollideWallTic()
    {

        float checkDist = 0.5f;
        //checkDist = (speed2.magnitude * Time.fixedDeltaTime) * 4;
        //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, -1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, -1) * checkDist), Color.blue);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * checkDist), Color.blue);
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
    private void CollideWallTicDie()
    {

        float checkDist = 0.7f;
        //checkDist = (speed2.magnitude * Time.fixedDeltaTime) * 4;
        //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, -1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, -1) * checkDist), Color.blue);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * checkDist), Color.blue);
        //Debug.Log(checkDist);
        for (float rot = 0; rot < 2; rot += 0.125f) //increments of 0.125 degrees collision rays
        {
            Vector3 colAngle = new Vector3(Mathf.Sin(rot * Mathf.PI), 0, Mathf.Cos(rot * Mathf.PI));
            if (Physics.Raycast(transform.position, transform.TransformDirection(colAngle), out touchRay, (checkDist * transform.localScale.x), 1))
            {
                actionMode = Modes.Dead;
            }
        }



    }
    private int CollideFloorTic()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), out touchRay, transform.localScale.y * 0.6f, 1))
        {
            //Debug.Log("Ray was cast downward, and we got a hit!");
            transform.position = touchRay.point;
            transform.Translate(transform.up * (transform.localScale.y * 0.48f), Space.World);
            if (vspeed < 0)
            {
                vspeed = 0;
            }
            if (touchRay.collider.gameObject.tag == "Respawn")
            {
                //gameObject.GetComponent<GameStateVariables>().health = 0; //restart if touching death surface
                actionMode = Modes.Knockback;
                return 1;
            }
            movingPlatform stoodOnPlatform = touchRay.transform.gameObject.GetComponent<movingPlatform>();
            if (stoodOnPlatform != null) { transform.position += stoodOnPlatform.GetMovementOffsetPerTic(); } //Character should move along with moving platforms they stand on.


        }
        else
        {
            //Debug.Log("Ray was cast downward, and we got a MISS! Switching player to freefall state");
            actionMode = Modes.Falling;

        }
        return 0;

    }
    private void CollideFloorFreeFallTic(bool holdingJump)
    {
        float vray = vspeed < 0 ? 0 - vspeed : 0;
        vray *= Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), out touchRay, (transform.localScale.y * (0.5f + vray)), 1))
        {
            //Debug.Log("Ray was cast downward, and we got a hit! Switching back to running mode");
            if (vspeed < 0) { actionMode = Modes.WalkingOrIdle; }
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
            if (holdingJump) { vspeed -= gravityForce * Time.fixedDeltaTime; } //lower jump when not holding key (hack)
        }
        transform.Translate((new Vector3(0, vspeed, 0) * transform.localScale.y) * Time.fixedDeltaTime);
        //hspeed = 0f;

    }
    private void CollideFloorDieTic(bool holdingJump)
    {
        float vray = vspeed < 0 ? 0 - vspeed : 0;
        vray *= Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), out touchRay, (transform.localScale.y * (0.5f + vray)), 1))
        {
            //Debug.Log("Ray was cast downward, and we got a hit! Switching back to running mode");
            if (vspeed < 0) { actionMode = Modes.Dead; }
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
            if (holdingJump) { vspeed -= gravityForce * Time.fixedDeltaTime; } //lower jump when not holding key (hack)
        }
        transform.Translate((new Vector3(0, vspeed, 0) * transform.localScale.y) * Time.fixedDeltaTime);
        //hspeed = 0f;

    }
}
