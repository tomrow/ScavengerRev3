using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndGoal : MonoBehaviour
{
    PlayerMovement player;
    Transform playerTf;
    float anima;
    //float revolutions;
    float revolutionsPerSecond = 620;
    float animLength = 4;
    public string nextlevel;
    AudioSource audioData;
    bool audioPlayed;
    public bool getAllCollectibles = false;
    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (!audioPlayed) { audioData.Play(); audioPlayed = true; }
            anima += Time.deltaTime*20;
            player.playerActionMode = PlayerMovement.Modes.ExtDisableControls2;//freeze the character in place
            playerTf.position = transform.position;
            playerTf.Find("body").Rotate(Vector3.up * (revolutionsPerSecond * Time.deltaTime));
            //playerTf.Find("body").localScale = new Vector3(1 / anima, anima / 2, 1 / anima); //slow spaghettify
            playerTf.Find("body").localScale += new Vector3(-8* Time.deltaTime, 8*Time.deltaTime, -8*Time.deltaTime);
            if(playerTf.Find("body").localScale.x < 0) 
            { playerTf.Find("body").localScale = Vector3.zero; }
            if (anima > animLength*10)
            {
                SceneManager.LoadScene(nextlevel);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(GameObject.FindGameObjectsWithTag("Plus").Length == 0 || !getAllCollectibles)
        {
            anima = 0;
            if (player == null || playerTf == null)
            {
                player = other.gameObject.GetComponent<PlayerMovement>();
                playerTf = other.gameObject.transform;
                audioPlayed = false;
            }
            else if (player != null)
            {
                player.playerActionMode = PlayerMovement.Modes.ExtDisableControls;//freeze the character in place
            }
        }
    }
}
