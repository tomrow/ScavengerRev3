using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeSawPlatformCharacterInteract : MonoBehaviour
{
    GameObject playerCharacter;
    float playerMass;
    float playerFallVel;
    PlayerMovement foundPlayerMovement;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        playerMass = 1f;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "PlrGndProbe")
        {
            
            
            playerFallVel = foundPlayerMovement.vspeed > 0 ? (foundPlayerMovement.vspeed * playerMass) + playerMass : 0;
            rb.AddForceAtPosition(Vector3.down * playerFallVel, other.transform.position, ForceMode.Force);
            Debug.Log(other.gameObject.tag);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "PlrGndProbe")
        {
            playerCharacter = other.gameObject.transform.parent.parent.gameObject; //This should be the actual Player gameobject rather than the playergroundprobe
            foundPlayerMovement = playerCharacter.GetComponent<PlayerMovement>();
        }
    }
}
