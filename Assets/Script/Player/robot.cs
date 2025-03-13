using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robot : MonoBehaviour
{

    public float stunTimer;
    public float corruptionCoolDownTimer;
    public GameObject blob;
    PlayerMovement pm;
    public float ballsAmt;
    public float ballsIncAmt = 0.2f;
    bool pickedUp;
    bool oldBtn;
    GameObject pickedUpBy;
    characterMulti cm;
    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        ballsAmt =0;
        cm=GameObject.Find("characterMulti").GetComponent<characterMulti>();
    }
    private void Update()
    {
        stunTimer -= Time.deltaTime;
        corruptionCoolDownTimer -= Time.deltaTime;
        if (stunTimer < 0)
        {
            stunTimer = 0;
        }
        if (corruptionCoolDownTimer < 0)
        {
            corruptionCoolDownTimer = 0;
        }
        ballsAmt = (transform.childCount * ballsIncAmt)/5f;
        if (transform.childCount > 5)
        {
            GameStateVariables.health -= (transform.childCount / 30) * Time.deltaTime;
        }
        if(pm.stunTimer <= 0.03f) { pickedUpBy = null; }
        if (pickedUpBy != null) 
        {
            transform.position = pickedUpBy.transform.position + (Vector3.up*2.0f);
        }

    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Touch!");
        if (other.gameObject.tag == "emp")
        {
            stunTimer = 10;
            corruptionCoolDownTimer = 10;
            //Instantiate(platform, transform.position, Quaternion.identity);
            //Destroy(gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "corruption")
        {
            
            if ((corruptionCoolDownTimer <= 0)) 
            {
                GameObject newBlob = Instantiate(blob, transform);
                newBlob.transform.position = other.ClosestPoint(transform.position);
                corruptionCoolDownTimer += 0.2f;
                cm.blobWideness = 0.3f;
                for (float i = 0; i <= ballsAmt; i++) { cm.blobWideness += (0.4f / ballsAmt > 1 ? ballsAmt : 1); }
                //ballsAmt += ballsIncAmt;
            }
            //Instantiate(platform, transform.position, Quaternion.identity);
            //Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player")
        {
            Debug.Log("player found within pickup range");
            PlayerMovement pm_o= other.gameObject.GetComponent<PlayerMovement>();
            Debug.Log(pm_o != null); Debug.Log(pm.stunTimer >= 0.03f); Debug.Log(Input.GetAxis("Fire2") > 0.5f);
            bool justDown = !oldBtn && Input.GetAxis("Fire2") > 0.5f;
            if (pm_o != null && pm.stunTimer >= 0.03f && justDown)
            {
                if (pickedUpBy == null) { pickedUpBy = other.gameObject; }
                else 
                {
                    Debug.Log("Drop robot.");
                    pm.knockBackDir = Vector3.zero;
                    pm.playerActionMode = PlayerMovement.Modes.Falling;
                    pickedUpBy = null;
                }

            }
            oldBtn = Input.GetAxis("Fire2") > 0.5f;

        }
    }
}
