using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



public class characterMulti : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject sGunBlast;
    public GameObject sGunBlastParticles;
    public int activeCharacter;
    public float blobWideness;
    public Transform cameraT;
    public GameObject platform;
    float platformtimer;
    PlayerMovement robotLast;
    PlayerMovement phys;
    robot robotData;
    LineRenderer lineRenderer;
    Material line;
    float scrollTimer;
    robot robotDataLast;
    float glitchTimer;
    public PlayerMovement.CameraMode currentCameraMode = PlayerMovement.CameraMode.Chase;
    public bool OldOverheadCamera;
    private void Start()
    {
        robotLast = GameObject.Find("robot").GetComponent<PlayerMovement>();
        robotData = characters[activeCharacter].GetComponent<robot>();
        if (robotData != null) 
        { 
            robotLast = robotData.gameObject.GetComponent<PlayerMovement>();
            robotDataLast = robotLast.gameObject.GetComponent<robot>();
        }
        phys = characters[activeCharacter].GetComponent<PlayerMovement>();
        lineRenderer = GetComponent<LineRenderer>();
        line = lineRenderer.material;
        scrollTimer = 0;
        phys.currentCameraMode = PlayerMovement.CameraMode.Chase;
        phys.cameraLooksAtCharacter = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, characters[0].transform.position);
            lineRenderer.SetPosition(1, characters.Length > 1 ? characters[1].transform.position : characters[0].transform.position);
            lineRenderer.enabled = characters.Length > 1;
            scrollTimer += Time.deltaTime;
            if (scrollTimer > 1) { scrollTimer -= 1; }
            line.mainTextureOffset = new Vector2(scrollTimer, 0);
            if (blobWideness > 2.5f) { blobWideness -= Time.deltaTime; }
        }
        


        if (phys.stunTimer <= 0.01f)
        {
            phys.Horizontal = Input.GetAxis("Horizontal"); //pipe controller input into active player
            phys.Vertical = Input.GetAxis("Vertical"); //pipe controller input into active player
            phys.Fire1 = Input.GetAxis("Fire1"); //pipe controller input into active player
            phys.Fire2 = Input.GetAxis("Fire2"); //pipe controller input into active player
            phys.RightStickHorizontal = Input.GetAxis("RightStickHorizontal"); //pipe controller input into active player
            phys.RightStickVertical = Input.GetAxis("RightStickVertical"); //pipe controller input into active player

            //if (robotData == null || robotData.stunTimer <= 0)
            //{
            //    characters[activeCharacter].transform.position += (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 3);
            //    characters[activeCharacter].transform.eulerAngles = new Vector3(0, Mathf.Atan2(stick.x, stick.y) * Mathf.Rad2Deg, 0);
            //}

            if (robotData != null) //not human
            {
                //Debug.Log(phys.gameObject.transform.Find("body").forward);
                if (Input.GetButtonDown("Fire2"))
                {
                    GameObject blast = Instantiate(sGunBlast, characters[activeCharacter].transform.position, characters[activeCharacter].transform.Find("body").rotation);
                    Instantiate(sGunBlastParticles, characters[activeCharacter].transform.position, characters[activeCharacter].transform.Find("body").rotation);
                    phys.stunTimer = 20;
                    phys.playerActionMode = PlayerMovement.Modes.Knockback;
                    //phys.knockBackDir = (phys.gameObject.transform.Find("body").TransformPoint(Vector3.forward) - phys.gameObject.transform.position);
                    Debug.Log("td direction:" + phys.gameObject.transform.Find("body").TransformDirection(Vector3.forward).ToString());
                    Debug.Log("other direction:" + phys.gameObject.transform.Find("body").forward.ToString());
                    phys.knockBackDir = (phys.gameObject.transform.Find("body").forward * -0.4f); // - phys.gameObject.transform.position);
                    phys.knockBackDir.y = -0.5f;
                    phys.knockBackDir = phys.knockBackDir.normalized;
                }
            }
            else
            {
                if (phys.playerActionMode == PlayerMovement.Modes.SpinAttack && !phys.SpinAttackOnGnd && robotLast.stunTimer >= 0.5f)
                {
                    platformtimer += Time.deltaTime;
                    if (GameStateVariables.score > 0 && platformtimer > 0.1f)
                    {
                        platformtimer = 0f;
                        GameStateVariables.score -= 1;
                        GameObject b = Instantiate(platform, phys.gameObject.transform.position - (Vector3.up * 1.5f), Quaternion.identity);
                    }
                }
                else { platformtimer = 0f; }
            }

        }
        if (Input.GetButtonDown("Fire3"))
        {
            phys.Horizontal = 0;
            phys.Vertical = 0;
            phys.Fire1 = 0;
            phys.Fire2 = 0;
            phys.RightStickHorizontal = 0;
            phys.RightStickVertical = 0; //clear out old input so the previous character stands still and doesnt run off
            phys.currentCameraMode = PlayerMovement.CameraMode.DoNothing;
            phys.cameraLooksAtCharacter = false;
            //phys.currentCameraMode = PlayerMovement.CameraMode.DoNothing;
            activeCharacter++; //switch character
            if (activeCharacter >= characters.Length)
            { activeCharacter = 0; } //loop back to start
            robotData = characters[activeCharacter].GetComponent<robot>();
            if (robotData != null)
            {
                robotLast = robotData.gameObject.GetComponent<PlayerMovement>();
                robotDataLast = robotLast.gameObject.GetComponent<robot>();
            }
            phys = characters[activeCharacter].GetComponent<PlayerMovement>();
            phys.currentCameraMode = PlayerMovement.CameraMode.Chase;
            phys.cameraLooksAtCharacter = true;
        }
        if (OldOverheadCamera)
        {
            cameraT.position = characters[activeCharacter].transform.position;
            cameraT.eulerAngles = Vector3.right * 45;//Todo move camera to hover to average of player positions when they're in a vicinity of each other
            cameraT.Translate(Vector3.forward * -6);
        }
        
        if (robotDataLast.transform.childCount > UnityEngine.Random.Range(4, 100))
        {
            robotLast.Horizontal = UnityEngine.Random.Range(-1f, 1f);
            robotLast.Vertical = UnityEngine.Random.Range(-1f, 1f);
            robotLast.Fire1 = UnityEngine.Random.Range(0f, 1f);
            robotLast.Fire2 = UnityEngine.Random.Range(0f, 1f);
        }
        else
        {
            if (robotData != null)
            {
                robotLast.Horizontal = Input.GetAxis("Horizontal"); //pipe controller input into active player
                robotLast.Vertical = Input.GetAxis("Vertical"); //pipe controller input into active player
                robotLast.Fire1 = Input.GetAxis("Fire1"); //pipe controller input into active player
                robotLast.Fire2 = Input.GetAxis("Fire2"); //pipe controller input into active player
                robotLast.RightStickHorizontal = Input.GetAxis("RightStickHorizontal"); //pipe controller input into active player
                robotLast.RightStickVertical = Input.GetAxis("RightStickVertical"); //pipe controller input into active player }
            }
            else
            {
                robotLast.Horizontal = 0;
                robotLast.Vertical = 0;
                robotLast.Fire1 = 0;
                robotLast.Fire2 = 0;
                robotLast.RightStickHorizontal = 0;
                robotLast.RightStickVertical = 0;
            }
        }

    }
}
