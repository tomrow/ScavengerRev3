using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class JezBehaviour : MonoBehaviour
{
    enum Card
    { up = 0,
    down = 1,
    left = 2,
    right= 3}
    GameObject player;
    GameObject selector;
    ScavengerPersistentData persistentStorage;
    PlayerMovement playerMovement;
    Vector3 playerPosition;
    // Start is called before the first frame update
    bool[] previousFrameInput = new bool[4];
    bool[] currentFrameInput = new bool[4];
    bool[] justPressed = new bool[4];
    int selectedCategory = 0;
    string[] categories = new string[5];
    int selectedItem;
    [SerializeField] TextMeshProUGUI titleBar;
    [SerializeField] TextMeshProUGUI currentSelectedPartName;
    string[] itemsInCategory;
    bool playerHasBeenNearby;
    void Start()
    {
        for (int i = 0; i < 4; i++)
        { previousFrameInput[i] = false; currentFrameInput[i] = false; justPressed[i] = false; } //set all of these to false so we can log the previous input. 
        //this is so we can measure when the input is changed so we can implement a justpressed for analog axes.
        //I'm not 100% sure this is necessary but since I dont know what bools default to before theyre assigned a value im doing this anyway.
        playerHasBeenNearby = false;
        player = GameObject.FindWithTag("Player");
        if (player.GetComponent<PlayerMovement>() == null ) { Debug.Log("Why is this thing tagged as a player?"); Destroy(gameObject); }
        playerMovement = player.GetComponent<PlayerMovement>();
        selector = GameObject.Find("/Canvas/SelectPlayer");
        if (selector == null) { Debug.Log("Where is the UI?"); Destroy(gameObject); }
        selector.transform.localScale = Vector3.zero;
        persistentStorage = GameObject.FindWithTag("ScavengerPersistentStorage").GetComponent<ScavengerPersistentData>();
        categories[0] = "Characters";
        categories[1] = "Arms";
        categories[2] = "Legs";
        categories[3] = "Torsos";
        categories[4] = "Heads";
        currentSelectedPartName.text = GetSelectionText(selectedCategory, selectedItem);
        titleBar.text = "Select a part to use and push A\n<" + categories[selectedCategory] + ">";

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProximityCheck();
        if(playerMovement.playerActionMode == PlayerMovement.Modes.Talking && Vector3.Distance(transform.position, player.transform.position) < 8)
        {
            selector.SetActive(true);
            UITic();
        }
    }

    private void UITic()
    {
        PopulateArrayWithInput();
        //select category
        SwitchCategoryOnKbd();
        //select items in category
        SelectItemFromListOnKbd();


    }

    private void SelectItemFromListOnKbd()
    {
        selectedItem += justPressed[(int)Card.down] ? 1 : 0;
        selectedItem += justPressed[(int)Card.up] ? -1 : 0;
        //Debug.Log(selectedItem);
        selectedItem = (int)Math.Clamp(selectedItem, 0, persistentStorage.boxes);
        if (justPressed[(int)Card.up] || justPressed[(int)Card.down])
        {
            currentSelectedPartName.text = GetSelectionText(selectedCategory, selectedItem);
        }
        if (Input.GetAxis("Fire1") > 0.5f)
        {
            ConfirmSelection(selectedCategory, GetSelectionText(selectedCategory, selectedItem));
        }
    }

    private void SwitchCategoryOnKbd()
    {
        selectedCategory += justPressed[(int)Card.right] ? 1 : 0;
        selectedCategory += justPressed[(int)Card.left] ? -1 : 0;
        while (selectedCategory < 0) { selectedCategory += 5; }
        while (selectedCategory > 4) { selectedCategory -= 5; } //keep it within bounds
        if (justPressed[(int)Card.left] || justPressed[(int)Card.right])
        {
            selectedItem = 0;
            currentSelectedPartName.text = GetSelectionText(selectedCategory, selectedItem);
            titleBar.text = "Select a part to use and push A\n<" + categories[selectedCategory] + ">";
        }
    }

    private void PopulateArrayWithInput()
    {
        for (int i = 0; i < 4; i++)
        {
            previousFrameInput[i] = currentFrameInput[i];
        }
        currentFrameInput[(int)Card.up] = Input.GetAxis("Vertical") > 0.5f;
        currentFrameInput[(int)Card.down] = Input.GetAxis("Vertical") < -0.5f;
        currentFrameInput[(int)Card.right] = Input.GetAxis("Horizontal") > 0.5f;
        currentFrameInput[(int)Card.left] = Input.GetAxis("Horizontal") < -0.5f;
        for (int i = 0; i < 4; i++)
        { justPressed[i] = currentFrameInput[i] && !previousFrameInput[i];
            //Debug.Log(currentFrameInput[i] && !previousFrameInput[i]);
        }
    }

    private void ConfirmSelection(int sel, string v)
    {
        switch (sel)
        {
            case 1:
                persistentStorage.currentArms = v; break;
            case 2:
                persistentStorage.currentLegs = v; break;
            case 3:
                persistentStorage.currentTorso = v; break;
            case 4:
                persistentStorage.currentHead = v; break;
            default:
                persistentStorage.currentCharacter = v; break;
        }
        playerMovement.playerActionMode = PlayerMovement.Modes.ReloadCharacter;
        selector.transform.localScale = Vector3.zero;
    }

    private string GetSelectionText(int cat, int sel)
    {
        switch (selectedCategory)
        {
            case 1:
                return persistentStorage.Arms[sel];break;
            case 2:
                return persistentStorage.Legs[sel]; break;
            case 3:
                return persistentStorage.Torsos[sel]; break;
            case 4:
                return persistentStorage.Heads[sel]; break;
            default:
                return persistentStorage.Characters[sel]; break;
        }
    }
    private int GetCategoryLength(int cat)
    {
        switch (selectedCategory)
        {
            case 1:
                return persistentStorage.Arms.Length; break;
            case 2:
                return persistentStorage.Legs.Length; break;
            case 3:
                return persistentStorage.Torsos.Length; break;
            case 4:
                return persistentStorage.Heads.Length; break;
            default:
                return persistentStorage.Characters.Length; break;
        }
    }
    private void ProximityCheck()
    {
        playerPosition = player.transform.position;
        //Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        if (Vector3.Distance(transform.position, player.transform.position) < 8 && (playerMovement.playerActionMode == PlayerMovement.Modes.SpinAttack) )
        {
            //player has approached just now; activate dialogue
            playerMovement.playerActionMode = PlayerMovement.Modes.Talking;
            selector.transform.localScale = Vector3.one;
        }

        playerHasBeenNearby = (Vector3.Distance(transform.position, player.transform.position) < 2);
    }
}
