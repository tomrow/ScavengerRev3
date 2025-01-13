using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class control : MonoBehaviour
{
    float mousex;
    float mousey;
    // Start is called before the first frame update.
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mousex = Input.GetAxis("Mouse X");
        mousey = Input.GetAxis("Mouse Y");
        Debug.Log(mousey);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        transform.Rotate(0-mousey,mousex,0,Space.Self);
    }
}
