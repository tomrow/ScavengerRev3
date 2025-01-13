using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;


public class BloodyAi : MonoBehaviour
{
    //Variable declarations
    NavMeshAgent nav; //navigation mesh
    Transform player; // contains player position
    GameObject playerObject;
    public Transform bonesModel;
    Renderer bbrenderer;
    public Animator animatorMesh;
    AnimatorStateInfo animatorStateInfo;
    public int animMode;
    float timer;
    bool hiding = true;
    [SerializeField] float speed;
    RaycastHit touchRay;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player"); //search for player object and get its position properties when found
        player = playerObject.transform;
        //bonesModel = transform.Find("bbModel");
        bbrenderer = GetComponent<Renderer>();
        //animatorMesh = GetComponentInChildren<Animator>();
        animatorStateInfo = animatorMesh.GetCurrentAnimatorStateInfo(0);
    }

    // Update is called once per frame
    void Update()
    {
        //nav.SetDestination(player.position); //tells object to seek the player coordinates
        if (hiding)
        {
            timer = 0;
            bonesModel.localScale = Vector3.zero;
            float zpos = Random.Range(300, 1800);
            float xpos = Random.Range(1300, 1300+Mathf.Cos((zpos / 750) * Mathf.PI) * -900);
            xpos -= ((zpos - 300) * 0.8f);
            //slant it for map
            //^ get distance from center of zaxis
            transform.position = new Vector3(xpos, -1000, zpos);
            hiding = false;
            if (Physics.Raycast(transform.position, Vector3.up, out touchRay, 3000f, 1)){ transform.position = touchRay.point; hiding = inDisplayBounds(Camera.main.WorldToViewportPoint(transform.position)); } else { hiding = true; }
            //snap to floor, or close enough
            
        }
        else
        {
            if (!Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.up * -1, out touchRay, 3000f, 1)) { hiding = true; }
            //enter hiding if bloodybones somehow falls off the world
            bonesModel.localScale = Vector3.one;
            float dist = Vector3.Distance(transform.position, player.position);
            animMode = 0;
            bonesModel.localPosition = Vector3.one * -0.8f;
            if (inDisplayBounds(Camera.main.WorldToViewportPoint(transform.position)))
            { 
                if (dist > 300)
                { timer += Time.deltaTime; hiding = timer > 3; }
                if (30 < dist && dist < 300)
                { //go slow
                    nav.SetDestination(player.position); //tells object to seek the player coordinates
                    nav.speed = 2;
                }
                if (3 < dist && dist < 30)
                { //go fast
                    nav.SetDestination(player.position); //tells object to seek the player coordinates
                    nav.speed = 6;
                    
                }
                if (dist <= 3)
                { //pounce 
                    animMode = 1;
                    nav.SetDestination(player.position); //tells object to seek the player coordinates
                    bonesModel.localPosition = new Vector3(0, 2.5f, 0) ;
                    nav.speed = 16;
                }
            }
            else
            {
                { timer += Time.deltaTime; hiding = timer > 10; }
            }
            animatorMesh.SetInteger("mode", animMode);
            animatorMesh.SetFloat("speed", nav.velocity.magnitude);
            Debug.DrawLine(transform.position, player.position, Color.blue);
            speed = nav.velocity.magnitude;
        }


    }

    private bool inDisplayBounds(Vector3 viewpos)
    {
        if (viewpos.x < 0) {  return false; }
        if(viewpos.y < 0) {return false; }
        if(viewpos.z< 0) {return false; }
        if (viewpos.x > 1) { return false; }
        if (viewpos.y > 1) { return false; }
        return true;
    }
}