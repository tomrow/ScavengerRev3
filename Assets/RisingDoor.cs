using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RisingDoor : MonoBehaviour
{
    public GameObject[] buttons;
    bool active;
    float lerpTimer;
    public float speedMultiplier;
    public float openOffset;
    Vector3 origin;
    public float upDuration;
    float upTimer;
    public Vector3 moveDirection;
    public GameObject explosion;
    // Start is called before the first frame update
    bool exploded = false;
    bool explode;
    float scale;
    void Start()
    {
        origin = transform.localPosition;
        explode = explosion != null; //if there is an explosion prefab selected, treat the door as rigged to blow
        scale = transform.InverseTransformVector(Vector3.forward).magnitude;
    }
    
    // Update is called once per frame
    void Update()
    {
        active = false;
        foreach (GameObject button in buttons)
        {
            Button_ScrCustomObject btnData = button.GetComponent<Button_ScrCustomObject>();
            active = active || btnData.buttonDown;
            
        }

        if (active)
        {
            lerpTimer += Time.deltaTime * speedMultiplier;
            
            upTimer = 0;
        }
        else if ((upTimer > upDuration) && exploded == false) //do not close exploded doors
        {
            lerpTimer -= Time.deltaTime * speedMultiplier;
            lerpTimer = Mathf.Clamp01(lerpTimer);

        }
        else { upTimer += Time.deltaTime; }
        lerpTimer = Mathf.Clamp01(lerpTimer);
        if (lerpTimer > 0.8f) 
        {
            if (explode == true && exploded == false)
            { Instantiate(explosion, transform.position, Quaternion.identity); exploded = true; }
        }
        transform.localPosition = origin + (moveDirection * scale * openOffset * lerpTimer);
    }
}
