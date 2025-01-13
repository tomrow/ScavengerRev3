using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KINEMATICTOGGLER : MonoBehaviour
{
    bool inLeftHand;
    bool inRightHand;
    Rigidbody parentcollider;
    Renderer parentmaterial;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        parentcollider = GetComponentInParent<Rigidbody>();
        parentmaterial = GetComponentInParent<Renderer>();
        Debug.Log(Input.GetAxis("Fire2"));
        if (inLeftHand || inRightHand)
        {

            if (Input.GetButtonUp("Fire2"))
            {
                parentcollider.isKinematic = !(parentcollider.isKinematic);
                if (parentcollider.isKinematic)
                {
                    parentmaterial.material.SetColor("_Color", Color.red);

                }
                else
                {
                    parentmaterial.material.SetColor("_Color", Color.white);
                }


            }
        }

    }
    private void OnCollisionEnter(Collision Hand)
    {
        Debug.Log(Hand.gameObject.name);
        if (Hand.gameObject.name == ("LeftController"))
        {
            inLeftHand = true;
        }
        else if (Hand.gameObject.name == ("RightController"))
        {
            inRightHand = true;
        }
    }
    private void OnCollisionStay(Collision Hand)
    {
        Debug.Log(Hand.gameObject.name);
    }
    private void OnCollisionExit(Collision Hand)
    {
        if (Hand.gameObject.name == ("LeftController"))
        {
            inLeftHand = false;
        }
        else if (Hand.gameObject.name == ("RightController"))
        {
            inRightHand = false;
        }
    }

}