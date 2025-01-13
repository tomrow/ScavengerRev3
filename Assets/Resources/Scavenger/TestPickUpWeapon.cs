using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPickUpWeapon : MonoBehaviour
{
    public bool dropped = true;
    public bool dontMoveObjUp;
    public int collected = 0;
    public float swingTimer;
    public float swingAngle;
    [Tooltip("Sound effect to play for slashes")] public AudioClip AttackSoundEffect;
    PlayerMovement characterController;
    TrailRenderer trailRenderer;


    // Start is called before the first frame update.
    void Start()
    {
        
        swingTimer = -1f;
        if (!dontMoveObjUp)
        {
            transform.Translate(Vector3.up * 5);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dropped)
        {
            if ((swingTimer < 0) && (Input.GetAxis("Fire2") > 0.5f))
            { 
                swingTimer = 0f; 
                SpawnedMomentarySoundEffect.SpawnSnd(transform.position, AttackSoundEffect); 
            }
            transform.localEulerAngles = Vector3.up * 90f;
            if (swingTimer >= 0)
            { 
                if(characterController != null) { characterController.speed2 = Vector2.zero; }
                transform.localEulerAngles = new Vector3(0, 90 , -360 * (swingTimer - Mathf.Sqrt(swingTimer)));
                
                swingTimer += Time.fixedDeltaTime;
            }
            if (swingTimer >= 1) 
            {  
                swingTimer = -1f;
            }
            trailRenderer.emitting = ( swingTimer > 0f) && (swingTimer < 0.25f) ;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Player" && dropped == true)
        {
            
            characterController = collision.gameObject.GetComponent<PlayerMovement>();
            //gameObject.GetComponent<Collider>().enabled = false;
            //increment health by 1
            //collision.gameObject.GetComponent<GameStateVariables>().health += 1;
            //collision.gameObject.GetComponent<GameStateVariables>().score += 1;
            //if health bigger than maxhealth, then set the health to the max value, cancelling out the increase
            if (collision.gameObject.GetComponent<GameStateVariables>().health > collision.gameObject.GetComponent<GameStateVariables>().maxHealth) { collision.gameObject.GetComponent<GameStateVariables>().health = collision.gameObject.GetComponent<GameStateVariables>().maxHealth; }

            transform.localScale = Vector3.zero;
            collected += 1;
            //play the sound effect
            collision.gameObject.transform.Find("Data").Find("Sound").Find("PlusCollect").gameObject.GetComponent<AudioSource>().Play();
            transform.parent = collision.gameObject.transform.Find("body").Find("Grabber"); //get the part to parent the weapon to
            transform.localScale = Vector3.one; //reset the scaling, rotation and position to the same as new parent
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            dropped = false;
            // Here should increase the score and health
            //Destroy(this.gameObject);
            trailRenderer = transform.Find("TestSwordBlade").gameObject.GetComponent<TrailRenderer>();
            
        }
        
    }
}
