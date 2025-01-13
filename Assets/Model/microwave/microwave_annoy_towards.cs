using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class microwave_annoy_towards : MonoBehaviour
{
    Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Player.position);
        transform.eulerAngles = Vector3.Scale(transform.eulerAngles, Vector3.up);
    }
}
