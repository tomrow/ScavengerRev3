using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

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
    public float charge;
    public float chargeMax;
    public int minChildren;
    ParticleSystem.EmissionModule _emissionModule;
    public TextMeshProUGUI chargeDisplay;
    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        ballsAmt =0;
        cm=GameObject.Find("characterMulti").GetComponent<characterMulti>();
        minChildren = transform.childCount;
        _emissionModule = GetComponent<ParticleSystem>().emission;
        //_emissionModule.rateOverTimeMultiplier = 0;
        charge = chargeMax;
    }
    private void Update()
    {
        chargeDisplay.text = Convert.ToString((int)charge);
        _emissionModule.rateOverTime = Mathf.Clamp01(1-(charge/chargeMax)) * 600;
        _emissionModule.enabled = charge < chargeMax;
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
        if (transform.childCount > minChildren)
        {
            GameStateVariables.health -= (transform.childCount / 50) * Time.deltaTime;
            charge -= (transform.childCount / 20) * Time.deltaTime;
        }
        if (transform.childCount <= minChildren)
        {
            charge += Time.deltaTime;
        }
        if (charge <= 0)
        {
            charge = 0;
            pm.stunTimer = 5;
            if (pm.playerActionMode != PlayerMovement.Modes.Stun && pm.playerActionMode != PlayerMovement.Modes.StunFalling && pm.playerActionMode != PlayerMovement.Modes.Knockback)
            {
                pm.playerActionMode = PlayerMovement.Modes.Knockback;
                //phys.knockBackDir = (phys.gameObject.transform.Find("body").TransformPoint(Vector3.forward) - phys.gameObject.transform.position);
                pm.knockBackDir = (gameObject.transform.Find("body").forward * -0.4f); // - phys.gameObject.transform.position);
                pm.knockBackDir.y = -0.5f;
                pm.knockBackDir = pm.knockBackDir.normalized * 0.4f;
            }
        }
        charge = Mathf.Clamp(charge, 0, chargeMax);
        if (pm.stunTimer <= 0.03f) { pickedUpBy = null; }
        if (pickedUpBy != null) 
        {
            transform.position = pickedUpBy.transform.position + (Vector3.up*2.0f);
        }
        cm.blobWideness = 0.3f;
        for (float i = 0; i <= ballsAmt; i++) { cm.blobWideness += (0.4f / ballsAmt > 1 ? ballsAmt : 1); }

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
