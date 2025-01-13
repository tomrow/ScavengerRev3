using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerPersistentData : MonoBehaviour
{
    public string[] Heads;
    public string[] Torsos;
    public string[] Arms;
    public string[] Legs;
    public string[] Characters;
    public int caps;
    public int boxes;
    public string currentCharacter;
    public string currentHead;
    public string currentTorso;
    public string currentArms;
    public string currentLegs;
    public int reflectionSmoothness;
    public bool fullScreen;
    // Start is called before the first frame update.
    void Start()
    {
        Object.DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
