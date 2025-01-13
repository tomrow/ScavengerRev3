using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMatAnimation : MonoBehaviour
{
    public GameObject frame1;
    public GameObject frame2;
    public GameObject frame3;
    public GameObject frame4;
    int counter;
    int counter2;
    public int animationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        frame1 = transform.Find("goal1").gameObject;
        frame2 = transform.Find("goal2").gameObject;
        frame3 = transform.Find("goal3").gameObject;
        frame4 = transform.Find("goal4").gameObject;
        counter = 0;
        counter2 = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        counter++;
        if (counter >= animationSpeed)
        {
            counter = 0;
            counter2++;
    
        }
        if (counter2 > 3) { counter2 = 0; }

        if (counter2 == 0) { frame1.SetActive(true); frame2.SetActive(false); frame3.SetActive(false); frame4.SetActive(false); }
        if (counter2 == 1) { frame1.SetActive(false); frame2.SetActive(true); frame3.SetActive(false); frame4.SetActive(false); }
        if (counter2 == 2) { frame1.SetActive(false); frame2.SetActive(false); frame3.SetActive(true); frame4.SetActive(false); }
        if (counter2 == 3) { frame1.SetActive(false); frame2.SetActive(false); frame3.SetActive(false); frame4.SetActive(true); }
    }
}
