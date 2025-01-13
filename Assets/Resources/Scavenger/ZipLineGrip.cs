using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.Experimental;
using UnityEngine;

public class ZipLineGrip : MonoBehaviour
{
    public Vector3 handleOffset = Vector3.zero;
    Quaternion characterIdentity;
    public float speed = 15f;
    public Transform initialNode;
    [SerializeField]Transform currentNode;
    ZipLineNode currentNodeData;
    [SerializeField] Vector3 nextNodePos;
    [SerializeField] Vector3 distanceToNext;
    public Transform GlamourCameraPosition;
    public bool UseGlamourCameraPosition = false;
    bool moving = false;
    GameObject currentCharacter;
    PlayerMovement currentCharacterState;
    [SerializeField] float exitVSpeed = 20f;
    [Tooltip("Units per second per second")]public float accelleration = 0.1f;
    float currentSpeed;
    MeshRenderer currentMeshRenderer;

    #region sound
    AudioSource scrapingSound;
    AudioSource startSound;
    AudioSource endSound;
    #endregion

    void SetupGrip()
    {
        currentNode = initialNode;
        currentNodeData = currentNode.gameObject.GetComponent<ZipLineNode>();
        nextNodePos = currentNodeData.nextNode.position;
        transform.LookAt(nextNodePos);
        //transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        transform.position = currentNode.transform.position;
        scrapingSound.Stop();
        scrapingSound.pitch = 6;
    }
    void Start()
    {
        currentMeshRenderer = GetComponent<MeshRenderer>();
        scrapingSound = GetComponent<AudioSource>();
        startSound = transform.Find("clang").gameObject.GetComponent<AudioSource>();
        endSound = transform.Find("click").gameObject.GetComponent<AudioSource>();
        SetupGrip();
    }

    private void FixedUpdate()
    {
        currentMeshRenderer.enabled = !moving;
        
        if (moving)
        {
            transform.LookAt(nextNodePos);
            currentSpeed += accelleration * Time.fixedDeltaTime;
            scrapingSound.pitch += 0.2f * Time.fixedDeltaTime;
            if (currentCharacterState == null) { moving = stopMoving(); return; }
            if(!NodeIsGoodAndNotTerminal(currentNode)) { moving = stopMoving(); endSound.Play(); return; } //no next node or no node script? consider it an end
            float pendingMovement = currentSpeed * Time.fixedDeltaTime;
            distanceToNext =  nextNodePos - transform.position;
            while (distanceToNext.magnitude < pendingMovement) 
            { 
                NextNode();
                pendingMovement -= distanceToNext.magnitude;
                distanceToNext = transform.position - nextNodePos;
                transform.LookAt(nextNodePos);
            }
            if (NodeIsGoodAndNotTerminal(currentNode)) 
            {
                Debug.Log(distanceToNext.magnitude);
                distanceToNext = transform.position - nextNodePos;
                transform.Translate(Vector3.forward * (pendingMovement), Space.Self);
            }
            currentCharacter.transform.position = transform.position;
            currentCharacter.transform.rotation = transform.rotation;

        }
        else { transform.Rotate(0, 180*Time.fixedDeltaTime, 0); }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject chk = other.gameObject;
        if (
            chk != null &&
            chk.GetComponent<PlayerMovement>() != null &&
            !moving
            ) 
        {
            currentSpeed = speed;
            currentCharacter = chk;
            currentCharacterState = chk.GetComponent<PlayerMovement>();
            moving = true;
            startSound.Play();
            currentCharacterState.playerActionMode = PlayerMovement.Modes.ExtDisableControls;
            if (UseGlamourCameraPosition)
            {
                currentCharacterState.currentCameraMode = PlayerMovement.CameraMode.ZipToLinear;
                currentCharacterState.CameraZipDurationSeconds = Time.fixedDeltaTime;
                currentCharacterState.CameraZipTarget = GlamourCameraPosition.position;
            }
            characterIdentity = chk.transform.rotation; //store this so we can return the character to its initial rotation later.
            scrapingSound.Play();
        }
    }
    bool NodeIsGoodAndNotTerminal(Transform subject)
    {
        ZipLineNode node = subject.gameObject.GetComponent<ZipLineNode>();
        if(node != null && node.nextNode != null) { return true; }
        return false;
    }
    void NextNode()
    {
        if (currentNodeData.nextNode != null)
        { 
            transform.position = currentNodeData.nextNode.transform.position;
            currentNode = currentNodeData.nextNode;
            currentNodeData = currentNode.GetComponent<ZipLineNode>();
            transform.LookAt(currentNodeData.nextNode.transform.position);
            nextNodePos = currentNodeData.nextNode.position;
        }
    }
    bool stopMoving()
    { 
        if (currentCharacterState != null) 
        { 
            currentCharacterState.playerActionMode = PlayerMovement.Modes.WalkingOrIdle;
            currentCharacterState.currentCameraMode = PlayerMovement.CameraMode.Chase;
            currentCharacterState.vspeed = exitVSpeed; //slight hop after getting off the zipline
            currentCharacter.transform.rotation = characterIdentity;
            SetupGrip();
        }
        return false; //change character state here so it doesnt change it every frame forever.
    }
}
