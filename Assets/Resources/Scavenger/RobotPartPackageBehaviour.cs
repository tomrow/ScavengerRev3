using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPartPackageBehaviour : MonoBehaviour
{
    public bool dontMoveObjUp;
    public int collected = 0;
    // Start is called before the first frame update.
    void Start()
    {
        if (!dontMoveObjUp)
        {
            transform.Translate(Vector3.up * 5);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, 0.1f, 0);
        if (collected > 0)
        {
            collected += 1;
        }
        if (collected > 10)
        {
            Destroy(gameObject);
        }

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && collected < 1)
        {
            gameObject.GetComponent<Collider>().enabled = false;
            //increment health by 1
            collision.gameObject.GetComponent<GameStateVariables>().health = collision.gameObject.GetComponent<GameStateVariables>().maxHealth;
            collision.gameObject.GetComponent<GameStateVariables>().boxes += 1;
            collision.gameObject.GetComponent<PlayerMovement>().playerActionMode = PlayerMovement.Modes.FallIntoSecretDance;
            //if health bigger than maxhealth, then set the health to the max value, cancelling out the increase
            if (collision.gameObject.GetComponent<GameStateVariables>().health > collision.gameObject.GetComponent<GameStateVariables>().maxHealth) { collision.gameObject.GetComponent<GameStateVariables>().health = collision.gameObject.GetComponent<GameStateVariables>().maxHealth; }

            transform.localScale = Vector3.zero;
            collected += 1;
            //play the sound effect
            collision.gameObject.transform.Find("Data").Find("Sound").Find("BoxCollect").gameObject.GetComponent<AudioSource>().Play();
            // Here should increase the score and health
            //Destroy(gameObject);
        }
    }
}
