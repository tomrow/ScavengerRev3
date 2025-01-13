using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimOnly : MonoBehaviour
{
    // Start is called before the first frame update.

    public Rigidbody something;
    float W = 0f ;
    float WA = -45f;
    float A = -90f ;
    float AS = -135f;
    float S = 180f;
    float SD = 135f;
    float D = 90f;
    float DW = 45f;
    float currentAngle;
    float angleIncrement;
    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;
    float workingAngle;
     
    float smooth = 0.3f;
    //float distance = 5.0f;
    float yVelocity = 0f;
        
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    //Fetch the Rigidbody from the GameObject with this script attached


    void FixedUpdate()

    {


        Animator Shittiest_scripting_interface_ive_ever_used = GetComponent<Animator>();
        currentAngle = transform.eulerAngles.y;

        something = GetComponentInParent<Rigidbody>();
        /*if (Input.GetButton("Jump"))
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
        }*/
        if (Input.GetKey(KeyCode.A) & Input.GetKey(KeyCode.S))
        {
            Debug.Log("Detect key AS");
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, AS, ref yVelocity, smooth);

        }
        else if (Input.GetKey(KeyCode.S) & Input.GetKey(KeyCode.D))
        {
            Debug.Log("Detect key SD");
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, SD, ref yVelocity, smooth);

        }

        else if (Input.GetKey(KeyCode.D) & Input.GetKey(KeyCode.W))
        {
            Debug.Log("Detect key DW");
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, DW, ref yVelocity, smooth);

        }
        else if (Input.GetKey(KeyCode.A) & Input.GetKey(KeyCode.W))
        {
            Debug.Log("Detect key AW");
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, WA, ref yVelocity, smooth);

        }
        else if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("Detect key W");

            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, W, ref yVelocity, smooth);

        }
        else if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("Detect key A");
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, A, ref yVelocity, smooth);

        }

        else if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Detect key S");
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, S, ref yVelocity, smooth);

        }
        else if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("Detect key D");
            workingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, D, ref yVelocity, smooth);

        }
        transform.eulerAngles = new Vector3(0, workingAngle, 0);
        RaycastHit Grounded;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up * -1), out Grounded, 1))
        {
            Shittiest_scripting_interface_ive_ever_used.SetBool("ISGROUNDED", true);
            Debug.Log("Groundedtrue");
        }
        else
        {
            Shittiest_scripting_interface_ive_ever_used.SetBool("ISGROUNDED", false);
            Debug.Log("Groundedfalse");
        }

        if (something.mass * (something.velocity) == Vector3.zero)
        {
            Shittiest_scripting_interface_ive_ever_used.SetBool("ISMOVING", false);
            Debug.Log("Not Moving");
        }
        else
        {
            Shittiest_scripting_interface_ive_ever_used.SetBool("ISMOVING", true);
            Debug.Log("Moving");
        }
        if (something.velocity.magnitude > 0.01)
        { Shittiest_scripting_interface_ive_ever_used.SetFloat("SPEED", (something.mass * (something.velocity.magnitude))); }
        else { Shittiest_scripting_interface_ive_ever_used.SetFloat("SPEED", 1f); }
        //if (something.velocity.magnitude == 0)
        //{
        //    //Shittiest_scripting_interface_ive_ever_used.SetFloat("SPEED", 0.5f);
        //}
        // Genuine HATE // 
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
