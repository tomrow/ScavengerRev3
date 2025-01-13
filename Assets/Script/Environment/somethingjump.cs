using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class somethingjump : MonoBehaviour
{
    // Start is called before the first frame update.


    /*float W = 0f ;
    float WA = -45f;
    float A = -90f ;
    float AS = -135f;
    float S = 180f;
    float SD = 135f;
    float D = 90f;
    float DW = 45f;
    float currentAngle;
    float angleIncrement;*/
    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;
    float workingAngle;

    //float smooth = 0.3f;
    //float distance = 5.0f;
    //float yVelocity = 0f;
        
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }   
    //Fetch the Rigidbody from the GameObject with this script attached
       

    void FixedUpdate()

    {
        //currentAngle = transform.eulerAngles.y;
            if (Input.GetButton("Jump"))
            {
                //Apply a force to this Rigidbody in direction of this GameObjects up axis
                m_Rigidbody.AddForce(transform.up * m_Thrust);
            }

            if (Input.GetAxis("Vertical") != 0)
            {
            m_Rigidbody.AddForce(transform.forward * (m_Thrust * Input.GetAxis("Vertical")));
            }
            if (Input.GetAxis("Horizontal") != 0)
            {
            m_Rigidbody.AddForce(transform.right * (m_Thrust * Input.GetAxis("Horizontal")));
            }

            /*if (Input.GetKey(KeyCode.W))
            {
                
               
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, W, ref yVelocity, smooth);
               
            }
            else if (Input.GetKey(KeyCode.A))
            {
               
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, A, ref yVelocity, smooth);
               
            }
            else if (Input.GetKey(KeyCode.A) & Input.GetKey(KeyCode.W))
            {

            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, WA, ref yVelocity, smooth);

            }
            else if (Input.GetKey(KeyCode.S))
            {
            
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, S, ref yVelocity, smooth);
            
            }
            else if (Input.GetKey(KeyCode.A) & Input.GetKey(KeyCode.S))
            {

                workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, AS, ref yVelocity, smooth);

            }
            else if (Input.GetKey(KeyCode.S) & Input.GetKey(KeyCode.D))
            {

                workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, SD, ref yVelocity, smooth);

            }
            else if (Input.GetKey(KeyCode.D))
            {
                
                workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, D, ref yVelocity, smooth);
            
            }
            else if (Input.GetKey(KeyCode.D)& Input.GetKey(KeyCode.W))
            {

                workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, DW, ref yVelocity, smooth);

            }*/

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
