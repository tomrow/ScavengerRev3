using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textfader : MonoBehaviour
{
    public int maximum_non_fadout_time;
    float time_fading;
    // Start is called before the first frame update
    void Start()
    {
        if (maximum_non_fadout_time <= 0)
        {
            maximum_non_fadout_time = 10;

        }
    }

    // Update is called once per frame
    void Update()
    {
        time_fading += Time.deltaTime;
        if (maximum_non_fadout_time <= time_fading)
        {
            Destroy(gameObject);
        }
    }
}
