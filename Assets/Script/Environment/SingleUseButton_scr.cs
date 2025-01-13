using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUseButton_scr : MonoBehaviour
{
    
    float buttonmovement = 0.001f;
    public Rigidbody stuff;
    public bool verification;
    bool buttonDown;
    int buttonDownDuration;
    int lastknownplayer;
    // Start is called before the first frame update.
    void Start()
    {
        gameObject.AddComponent<Rigidbody>();
        stuff = GetComponent<Rigidbody>();
        stuff.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y > 0.8906592f)
        { transform.localPosition = new Vector3(0, 0.8906592f, 0); }
        if (transform.localPosition.y < 0.4906592f)
        { transform.localPosition = new Vector3(0, 0.4906592f, 0); }
        if (buttonDown)
        {
            buttonDownDuration ++ ;
            
            
                stuff.position += (Vector3.up * (buttonmovement * -1)) /2;

            
            if (buttonDownDuration > 3)
            {
                buttonDownDuration = 3;
            }
        }
        else
        {
            if (!buttonDown)
            {
                
                stuff.position += (Vector3.up * (buttonmovement * 1)) / 2;
                buttonDownDuration --;

                if (buttonDownDuration < 0)
                {
                    buttonDownDuration = 0;
                }

                if (transform.localPosition.y > 0.8906592f)
                { transform.localPosition = new Vector3(0,0.8906592f,0); }
                if (transform.localPosition.y < 0.4906592f)
                { transform.localPosition = new Vector3(0, 0.4906592f, 0); }

            }
        }
        
        Debug.Log(buttonDownDuration);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        /*the above debug log regularly returns the base to the button object. This is hopefully going to be a fix to any potential problems that causes.*/

        if(other.gameObject.name == "BUTTONBASE")
        {
            Debug.Log("FALSEALARM");
        }


        /* Checks if the interactor is the player */
        
        if (other.gameObject.name == ("PLAYER"))
        {
            buttonDown = true;
            Debug.Log("Player has interacted. SUCCESS!");
            

        }

    }
    private void OnTriggerExit(Collider other)
    {


     buttonDown = false;
            /*buttonDownDuration = 0;*/
    }
}
