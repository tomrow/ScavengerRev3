using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    float timer;
    public float timerDivision;
    public float positonMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime/timerDivision;
        while (timer>2*Mathf.PI)
        { timer -= 2 * Mathf.PI; }
        transform.localPosition = new Vector3(0, Mathf.Sin(timer) * positonMultiplier, 0);

    }
}
