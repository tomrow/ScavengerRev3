using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyFighterStumbleAnimation : MonoBehaviour
{
    float timer;
    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime/2;
        if (timer > 2) { timer -= 2; }
        Vector3 offset = new Vector3(Mathf.Sin(timer*Mathf.PI), Mathf.Cos(4*timer*Mathf.PI),0);
        transform.position = origin + (offset*400);
    }
}
