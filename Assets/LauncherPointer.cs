using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherPointer : MonoBehaviour
{
    private GameObject[] cableSegments = new GameObject[6];
    public GameObject baseCableType;
    public GameObject cableProjectileTemplate;
    Transform player;
    PlayerMovement phys;
    Vector2 rightStick;
    float cameraAngle;
    float aimAngle;
    bool ready;
    Quaternion aimAngle3D;
    float timer;
    GameObject projectile;
    LineRenderer lineRenderer;
    bool shootBackward;

    public float range;
    float reach;
    float gravityVelocity;
    RaycastHit rayout;
    CableConnector lastSegment;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        phys = player.GetComponent<PlayerMovement>();
        
        ready = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rightStick = new Vector2(phys.RightStickHorizontal, phys.RightStickVertical);
        cameraAngle = phys.cameraT.localEulerAngles.y;
        aimAngle = Mathf.Atan2(rightStick.x, 0-rightStick.y) * Mathf.Rad2Deg;
        
        aimAngle += cameraAngle;                 //calculate direction to fire relative to camera
        if (rightStick.magnitude < 0.1f) { aimAngle = phys.angleRun; aimAngle += shootBackward ? 180 : 0; } // if the stick isnt used, use character facing direction
                                                                                //Shoot 180 degrees away from player facing direction if shoot backwards is enabled
        aimAngle = aimAngle % 360;                                              //keep it within range, dont want floating point errors or trig weirdness now
        aimAngle3D.eulerAngles = new Vector3(0, aimAngle, 0);                   //this is fed into instantiate function
        transform.position = player.position + Vector3.up;
        transform.eulerAngles = new Vector3(90, aimAngle, 0);                   //add 90d to the x axis to have the quad face upward. the reverse side is invisible so we dont want to see it
        timer += Time.fixedDeltaTime;
        timer = timer % (2 * Mathf.PI);
        transform.localScale = Vector3.one * (1 + Mathf.Sin(timer) * 0.5f);    //pulsing animation
        if (lastSegment != null && lastSegment.hooked == null)
        {
            for (int i = 0; i < 6; i++)
            {
                if (cableSegments[i] != null)
                {
                    cableSegments[i].GetComponent<CableConnector>().UnHookObject();
                    Destroy(cableSegments[i]);
                }
            }
        }
        if (projectile == null && phys.Fire2 > 0)
        {
            for (int i = 0; i < 6; i++)
            {
                if (cableSegments[i] != null)
                {
                    cableSegments[i].GetComponent<CableConnector>().UnHookObject();
                    Destroy(cableSegments[i]);
                }
            }
            projectile = Instantiate(cableProjectileTemplate, player.position, aimAngle3D); reach = 0; Debug.Log("SHOOT"); lineRenderer = projectile.GetComponent<LineRenderer>();
        }
        else {  }

        if (projectile != null) //ideally this would be in its own script but it kept spawning with the script missing so we have to put up with this abominable hack
        {
            SimulateProjectile();
        }
        


    }

    private void SimulateProjectile()
    {
        lineRenderer.SetPosition(0, projectile.transform.position);
        lineRenderer.SetPosition(1, player.transform.position);
        Debug.DrawRay(projectile.transform.position, projectile.transform.forward, Color.green);
        if (Physics.Raycast(projectile.transform.position, Vector3.down, out rayout, transform.localScale.y / 2f))
        {
            gravityVelocity = 0;
            projectile.transform.position = rayout.point + (Vector3.up * projectile.transform.localScale.y * 0.45f);
            if (rayout.transform.tag == "Respawn")
            { Destroy(projectile); }
        }
        else
        {
            gravityVelocity += Time.fixedDeltaTime/4;
            projectile.transform.position += (Vector3.down * gravityVelocity);
        }
        if (reach > 0.25f && Physics.Raycast(projectile.transform.position + (Vector3.up * 0.5f), projectile.transform.forward, out rayout, transform.localScale.y / 2f))
        {
            HookedItem hookable = rayout.collider.gameObject.GetComponent<HookedItem>();
            Debug.Log(rayout.collider.gameObject.name);
            if (hookable != null) { GenerateCableSegments(hookable); }
            Destroy(projectile);
        }
        //projectile.transform.position = player.position + (projectile.transform.forward * reach * Time.fixedDeltaTime);
        projectile.transform.Translate(Vector3.forward * Time.fixedDeltaTime*8);
        Debug.Log(projectile.transform.position);
        reach += Time.fixedDeltaTime * 2;
        if (reach > range) { Destroy(projectile); }
    }

    public void GenerateCableSegments(HookedItem target)
    {
        Vector3 tgtPos = target.transform.position + (Vector3.up*0.5f) ;
        Vector3 distance = tgtPos - player.position;
        for (int i = 0; i < 6; i++)
        {
            cableSegments[i] = Instantiate(baseCableType, player.position + ((distance/5)*i), Quaternion.identity);
            CableConnector connector = cableSegments[i].GetComponent<CableConnector>();
            if (i > 0)
            {
                connector.cableRoot = cableSegments[i - 1].transform;
            }
            else { connector.cableRoot = player; }
            if (i == 5) { connector.HookObject(target); cableSegments[i].transform.localScale = target.transform.localScale; lastSegment = connector; }
        }

    }
    void GenerateProjectile()
    { }
}
