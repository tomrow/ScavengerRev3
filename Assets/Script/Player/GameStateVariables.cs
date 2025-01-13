using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GameStateVariables : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    public int score;
    public int boxes;
    //public GameObject scoreDisplay;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI boxesDisplay;
    public GameObject[] powerMeter;
    int goalCheckCounter;
    PlayerMovement playerMovementData;
    float healthPercentage;
    int powerMeterValue;
    ScavengerPersistentData scavengerPersistentData;

    // Start is called before the first frame update
    void Start()
    {
        goalCheckCounter = 0;
        health = maxHealth;
        playerMovementData = gameObject.GetComponent<PlayerMovement>();
        //scoreDisplay = GameObject.Find("ScoreDisplay");
        //scoreDisplayText = scoreDisplay.GetComponent<Text>();
        scavengerPersistentData = playerMovementData.persistentStorage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        scavengerPersistentData = playerMovementData.persistentStorage;
        if (health <= 0)
        {
            playerMovementData.playerActionMode = PlayerMovement.Modes.Death;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        scoreDisplay.text = Convert.ToString(score);
        boxesDisplay.text = Convert.ToString(boxes);
        scavengerPersistentData.boxes = boxes;
        scavengerPersistentData.caps = score;
        goalCheckCounter += 1;
        goalCheckCounter = goalCheckCounter % 60;
        if (goalCheckCounter == 0)
        {
            //if(GameObject.FindGameObjectsWithTag("Plus").Length < 1)
            //{
            //    SceneManager.LoadScene("b");
            //}
        }
        //Debug.Log(Mathf.Ceil((health / maxHealth) * 8)+1);
        healthPercentage = ((health*1.0f) / (maxHealth*1.0f)); //have to have a float somewhere in this otherwise it will try integer division and get wrong answer
        powerMeterValue = (int)Mathf.Ceil(healthPercentage * 8);
        for(int i = 0; i< powerMeter.Length; i++) { powerMeter[i].SetActive(false); } //set all the power meter graphics to disabled
        powerMeter[powerMeterValue].SetActive(true);                                  //only enable the applicable one

    }
}
