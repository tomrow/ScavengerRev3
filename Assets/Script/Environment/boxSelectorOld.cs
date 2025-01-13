using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boxSelectorOld : MonoBehaviour
{
    GameObject Collided;
    // Start is called before the first frame update.
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider PLAYTRIGGER)
    {
        if (PLAYTRIGGER.gameObject.name == "PLAYTRIGGER")
        {
            Debug.Log("Hit");
            SceneManager.LoadScene("SampleScene");
        }
    }
}
