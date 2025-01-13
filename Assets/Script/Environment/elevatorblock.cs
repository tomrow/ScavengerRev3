using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorblock : MonoBehaviour
    
{
    //creating variables that may be used.
    Rigidbody elevator;
    public Transform buttonTrigger;
    GameObject parentButton;
    //public GameObject ButtonControls;
    //public bool parentActive;
    Button_Scr Button_Scr; 
    //[SerializeField]
    //private Button_Scr Button_Scr;
    // Start is called before the first frame update..
    void Start()
    {
        //Calling objects that may be used.
        gameObject.AddComponent<Rigidbody>();
        elevator = GetComponent<Rigidbody>();
        elevator.isKinematic = true;
        parentButton = buttonTrigger.Find("BUTTONBASE (1)").gameObject;
        //Calling script. This needs correction, unless duplicate scripts are a nonissue.
        //Button_Scr = FindObjectOfType <Button_Scr>();
        Button_Scr = parentButton.GetComponentInParent<Button_Scr>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Button_Scr.buttonDown)
        {
            elevator.position += (Vector3.up * Button_Scr.buttonmovement);
        }
    }
}
