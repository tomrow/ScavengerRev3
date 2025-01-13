using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallPulse : MonoBehaviour
{
    public float Multiplier;
    float pulseTimer;
    Vector3 originScale;
    // Start is called before the first frame update
    void Start()
    {
        originScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        pulseTimer += Time.deltaTime*Multiplier;
        while (pulseTimer > 2 ) { pulseTimer -= 2; } //keep it within range
        transform.localScale = originScale * (1 - (Mathf.Sin(pulseTimer * Mathf.PI)/2) );
    }
}
