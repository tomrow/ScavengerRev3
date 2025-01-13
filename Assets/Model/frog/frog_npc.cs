using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frog_npc : MonoBehaviour
{
    Animator frogNPC;
    public int mode = 1;
    float gravity;
    RaycastHit touchRay;
    Vector2 speed2;
    Vector2 newSpeed2;

    Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        frogNPC = transform.Find("frog_animator").GetComponent<Animator>();
        Player = GameObject.FindWithTag("Player").transform;

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(Player.position);
        transform.eulerAngles = Vector3.Scale(transform.eulerAngles , Vector3.up);
        switch (mode)
        { case 0:
                newSpeed2 = new Vector2(Player.position.x - transform.position.x, Player.position.z - transform.position.z);
                speed2 = Vector2.Lerp(speed2, newSpeed2, 0.9f);
                if (speed2.magnitude < 0.6) { speed2 = Vector2.zero; } else { speed2 = speed2.normalized * Mathf.Clamp(speed2.magnitude, 0, 30); }
                CollideWallTic();
                transform.position += new Vector3(speed2.x, 0, speed2.y) * Time.fixedDeltaTime;
                CollideFloorTic();
                CollideWallTic();
                gravity = 0;
                break;
          case 5:
                gravity += 0.1f;
                newSpeed2 = new Vector2(Player.position.x - transform.position.x, Player.position.z - transform.position.z);
                speed2 = Vector2.Lerp(speed2, newSpeed2, 0.2f);
                //speed2 = new Vector2(Player.position.x - transform.position.x, Player.position.z - transform.position.z);
                if (speed2.magnitude < 0.6) { speed2 = Vector2.zero; } else { speed2 = speed2.normalized * Mathf.Clamp(speed2.magnitude, 0, 30); }
                CollideWallTic();
                transform.position += new Vector3(speed2.x, 0-gravity, speed2.y) * Time.fixedDeltaTime;
                if(gravity>0f)CollideFloorTic();
                CollideWallTic();
                break;
          case 1:
                speed2 = new Vector2(Player.position.x - transform.position.x, Player.position.z - transform.position.z);
                if (speed2.magnitude < 2) { mode = 0; }
                break;
            default: break;
        }
        frogNPC.SetInteger("mode", mode);
        frogNPC.SetFloat("speed", Time.fixedDeltaTime * (speed2.magnitude == 0 ? 0 : Mathf.Clamp(speed2.magnitude / Time.fixedDeltaTime, 0, 100)) );
        frogNPC.SetFloat("speed2", Time.fixedDeltaTime * (speed2.magnitude == 0 ? 1 : Mathf.Clamp(speed2.magnitude / Time.fixedDeltaTime, 1, 100)) );
    }


    private void CollideFloorTic()
    {
        float raylen = (Mathf.Abs(gravity) * Time.fixedDeltaTime) < 0.25f ? 0.25f : (gravity * Time.fixedDeltaTime);
        if(gravity < 0) { raylen *= -1; }
        Debug.DrawRay(transform.position, Vector3.up * (0 - raylen),Color.blue);
        
        if (Physics.Raycast(transform.position, Vector3.up * -1f, out touchRay, raylen, 1))
        {
            if (touchRay.transform.gameObject.tag == "Respawn")
            { transform.position = Camera.main.transform.position + (Vector3.up * -0.2f); gravity = 0; }
            mode = 0;
            transform.position = touchRay.point + (Vector3.up * 0.2f);
        }
        else { mode = 5; }
    }
    private void CollideWallTic()
    {

        float checkDist = speed2.magnitude * Time.fixedDeltaTime > 0.3f ? speed2.magnitude * Time.fixedDeltaTime : 0.3f;
        //checkDist = (speed2.magnitude * Time.fixedDeltaTime) * 4;
        checkDist += 0.1f;
        //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, -1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, -1) * checkDist), Color.blue);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 0) * checkDist), Color.red);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 1, 0) * checkDist), Color.green);        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * checkDist), Color.blue);
        //Debug.Log(checkDist);
        for (float rot = 0; rot < 2; rot += 0.125f) //increments of 0.125 degrees collision rays
        {
            Vector3 colAngle = new Vector3(Mathf.Sin(rot * Mathf.PI), 0, Mathf.Cos(rot * Mathf.PI));
            bool rc;
            rc = (Physics.Raycast(transform.position, colAngle, out touchRay, (checkDist * transform.localScale.x), 1));
            Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(colAngle), Color.red);
            Debug.DrawLine(transform.position, transform.position + colAngle, Color.green);
            Debug.DrawLine(transform.position, touchRay.point, Color.blue);
            if (rc)
            {
                //Debug.Log("Ray was cast horizontally, and we got a hit!");
                transform.position = touchRay.point;
                //Vector3 colnorm = touchRay.normal;
                //colnorm.y = 0;
                transform.Translate(colAngle * (transform.localScale.z * -0.4f), Space.World);

                //Make Frog jump if player is above it
                if (Player.position.y - transform.position.y > 1f)
                { gravity = -5; transform.position += Vector3.up * (gravity * Time.fixedDeltaTime); mode = 5; }
                if (speed2.magnitude > 3f)
                { gravity = -5; transform.position += Vector3.up * 0.2f; mode = 5; CollideWallTic(); speed2.Normalize(); speed2 *= 0.1f; }

            }
        }

    }
}
