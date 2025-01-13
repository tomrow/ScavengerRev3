using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lighting : MonoBehaviour
{
    GameObject lightToBeDeleted;
    Light lightAdded;
    public GameObject container;
    public float gametimeoutmaxtime;
    float gametimeouttime;
    public GameObject playerobj;
    Rigidbody Player_rigid;
    // Start is called before the first frame update.
    void Start()
    {
        lightToBeDeleted = GameObject.Find("Directional Light");
        Destroy(lightToBeDeleted);
        lightAdded = gameObject.AddComponent<Light>();
        lightAdded.type = LightType.Directional;
        container.transform.eulerAngles = new Vector3(29.759f, -72.401f, -60f);
        gametimeouttime = gametimeoutmaxtime;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("SampleScene"));

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Player_rigid = playerobj.GetComponent<Rigidbody>();
        Debug.Log(Player_rigid.velocity.magnitude);
        Vector2 stick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if ( Input.anyKey != true && stick.magnitude < 0.05f)
        {

            if (gametimeouttime < 0)
            {
                gametimeouttime = gametimeoutmaxtime;
                SceneManager.LoadScene("MenuScene");
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("MenuScene"));
            }
            else
            {
                gametimeouttime--;
            }
        }
        else if(Input.anyKey != false)
        {
            gametimeouttime = gametimeoutmaxtime;
        }
    }
}
