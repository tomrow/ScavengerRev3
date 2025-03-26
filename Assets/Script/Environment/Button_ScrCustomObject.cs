using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ScrCustomObject : MonoBehaviour
{
    
    public float  buttonmovement = 0.001f;
    public Rigidbody stuff;
    public bool verification;
    public bool buttonDown;
    bool buttonDown2;
    public string actuator;
    public float buttonFloor;
    public float buttonCeil;
    int buttonDownDuration;
    int lastknownplayer;
    AudioSource audioData;
    float riseTimer;
    // Start is called before the first frame update.
    void Start()
    {
        //gameObject.AddComponent<Rigidbody>();
        //stuff = GetComponent<Rigidbody>();
        //stuff.useGravity = false;
        audioData = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (buttonDown2)
        {
            buttonDownDuration++;
            transform.localPosition = new Vector3(0, buttonFloor, 0);
            buttonDown = true;

        }
        else
        {
            if (buttonDownDuration>32)
            { 
                buttonDownDuration = 0;
                transform.localPosition = new Vector3(0, buttonCeil, 0);
                buttonDown = false;
            }
        }
        riseTimer += Time.deltaTime;
        if(riseTimer > 0.3f && buttonDown2 == true) 
        {
            audioData.Play();
            buttonDown2 = false;
            riseTimer = 0;
        }
                       
        //Debug.Log(buttonDownDuration);
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);

        /*the above debug log regularly returns the base to the button object. This is hopefully going to be a fix to any potential problems that causes.*/

        if(other.gameObject.name == "BUTTONBASE")
        {
            Debug.Log("False alarm");
        }


        /* Checks if the interactor is the player */
        
        if (other.gameObject.name == actuator)
        {
            if(buttonDown2 == false) { audioData.Play(); }
            buttonDown2 = true;
            
            Debug.Log("Player has interacted. SUCCESS!");
            

        }
        riseTimer = 0;

    }
    private void OnTriggerExit(Collider other)
    {
        audioData.Play();
        if (other.gameObject.name == actuator)
        {
            buttonDown2 = false;
            /*buttonDownDuration = 0;*/
        }
    }
}
