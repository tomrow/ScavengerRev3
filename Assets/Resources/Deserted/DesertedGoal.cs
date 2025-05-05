using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DesertedGoal : MonoBehaviour
{
    characterMulti cmSystem;
    int charactersAtGoal;
    public string nextlevel;
    // Start is called before the first frame update
    void Start()
    {
        cmSystem = GameObject.Find("characterMulti").GetComponent<characterMulti>();
    }   

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        charactersAtGoal = 0;
        for (int i = 0; i < cmSystem.characters.Length; i++)
        {
            if (cmSystem.characters[i].GetComponent<PlayerMovement>().onDesertedGoal)
            { charactersAtGoal++; }
        }
        if (charactersAtGoal >= cmSystem.characters.Length)
        {
            SceneManager.LoadScene(nextlevel);
        }

    }
    void OnTriggerStay(Collider other)
    {
        PlayerMovement pm = other.gameObject.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.onDesertedGoal = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerMovement pm = other.gameObject.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.onDesertedGoal = false;
        }
    }
}
