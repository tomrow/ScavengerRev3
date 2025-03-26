using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingDoor : MonoBehaviour
{
    public GameObject[] buttons;
    bool active;
    float lerpTimer;
    public float speedMultiplier;
    public float openOffset;
    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
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
            lerpTimer = Mathf.Clamp01(lerpTimer);

        }
        else
        {
            lerpTimer -= Time.deltaTime * speedMultiplier;
            lerpTimer = Mathf.Clamp01(lerpTimer);

        }
        transform.position = origin + (Vector3.up * openOffset * lerpTimer);
    }
}
