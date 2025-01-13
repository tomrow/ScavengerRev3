using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boxSelector : MonoBehaviour
{
    public float Counter;
    public float Countermax;
    bool starttimer;
    bool playgroundTimer;
    bool ExitTimer;
    GameObject Collided;
    // Start is called before the first frame update.
    void Start()
    {
        starttimer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (starttimer||ExitTimer||playgroundTimer)
        {
            Counter += Time.deltaTime;
            Debug.Log(Counter);
        }
        if ((Counter >= Countermax) && starttimer)
        {
            
            SceneManager.LoadScene("Tutorial1");
        }
        if ((Counter >= Countermax) && ExitTimer)
        {

            Application.Quit();
        }
        if ((Counter >= Countermax) && playgroundTimer)
        {

            SceneManager.LoadScene("playground");
        }

    }
    private void OnTriggerEnter(Collider PLAYTRIGGER)
    {
        if (PLAYTRIGGER.gameObject.name == "TV_OLD")
        {
            starttimer = true;
            Debug.Log("Hit");
        }

        if (PLAYTRIGGER.gameObject.name == "umbrella_red")
        {
            ExitTimer = true;
            Debug.Log("Hit");
        }
        if (PLAYTRIGGER.gameObject.name == "tvtextured")
        {
            playgroundTimer = true;
            Debug.Log("Hit");
        }
    }
}
