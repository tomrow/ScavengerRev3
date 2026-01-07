using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class CableConnector : MonoBehaviour
{
    public Transform cableRoot;
    public float startTensionLength;
    public float weight;
    public float tensionLimit;
    public float tension;
    public float chainTension;
    public float slippery;
    float tensionBase;
    public HookedItem hooked;
    public Vector3 distance;
    LineRenderer cableSegment;
    float gravityVelocity;
    RaycastHit rayout;
    Vector3[] cardinals = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
    CableConnector croot;
    float gravityScale;

    // Start is called before the first frame update
    void Start()
    {
        gravityScale = 0.1f;
        if (hooked == null)
        { tensionBase = 1 / (weight); }//dont need to calculate this every frame
        else 
        { tensionBase = 1 / (weight + hooked.GetComponent<HookedItem>().weight); }
        cableSegment = GetComponent<LineRenderer>();
        croot = cableRoot.GetComponent<CableConnector>();
        //gameObject.tag = "Hook";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = cableRoot.position - transform.position;
        tension = tensionBase;
        tension *= (distance.magnitude - startTensionLength);
        tension = tension > 0 ? tension : 0; //prohibit negative values, we are making cables, not elastic rods
        if (croot != null) { chainTension = tension + croot.chainTension; } else { chainTension = tension; }
        CollideWithWalls();
        //transform.position += Vector3.Scale(distance.normalized * tension, Vector3.forward + Vector3.right);  //drag toward root, but not vertically
        if (Physics.Raycast(transform.position, Vector3.down, out rayout, transform.localScale.y))
        {
            Debug.DrawLine(transform.position, rayout.point, Color.green);
            gravityVelocity = 0;
            gravityScale = 1f;
            transform.position = rayout.point + (Vector3.up * transform.localScale.y * 0.9f);
            if (rayout.transform.tag == "Respawn")
            { transform.position = cableRoot.position; gravityVelocity = 0; }
            if (hooked != null && rayout.transform.tag == hooked.tag)
            {
                rayout.transform.GetComponent<MaterialPlatform>().consuming.Add((GameObject)Instantiate(Resources.Load(hooked.tag + "Eat"), hooked.transform.position, hooked.transform.rotation));
                GameObject junk = hooked.gameObject;
                UnHookObject(); 
                Destroy(junk);

            }
        }
        else 
        {
            gravityVelocity += (Time.fixedDeltaTime) * gravityScale /2;
            transform.position += (Vector3.down * gravityVelocity);
        }
        cableSegment.SetPosition(0, transform.position);
        cableSegment.SetPosition(1, cableRoot.position); //change this to midpoint collider position when done testing
        if(chainTension>tensionLimit)
        { 
            transform.position = cableRoot.position + (Vector3.Scale(distance, Vector3.forward + Vector3.right).normalized * startTensionLength);
            gravityVelocity = 0;
            ///if(transform.position.y >= cableRoot.position.y + (transform.localScale.y * 3))  //detach hooked object;
            UnHookObject();
        }
        if (hooked != null) { hooked.transform.position = transform.position;
            if (hooked.towParent == null)
                { UnHookObject(); }
            
        }
        
        Debug.DrawRay(transform.position, Vector3.up * tensionLimit, Color.blue); Debug.DrawRay(transform.position, Vector3.up * chainTension, Color.yellow); Debug.DrawRay(transform.position, Vector3.up * tension, Color.red); 

    }
    public void HookObject(HookedItem i)
    {
        hooked = i;
        hooked.towParent = this;
        tensionBase = 1 / (weight + hooked.GetComponent<HookedItem>().weight);
    }
    public void UnHookObject()
    {
        if (hooked != null)
        {
            Collider c = hooked.gameObject.GetComponent<Collider>();
            if (c != null)
            {
                c.enabled = true;
            }
        }
        hooked = null;
        tensionBase = 1 / (weight);
    }
    void CollideWithWalls()
    {
        Vector3 vel = (distance.normalized);
        vel.y = 0;
        float reach = transform.localScale.y+ tension;
        if (Physics.Raycast(transform.position + (Vector3.up * (transform.localScale.y / 2f)), vel.normalized, out rayout, reach))
        {
            Vector3 push = (vel.normalized * transform.localScale.y) * -0.9f;
            push += rayout.point;
            push.y = transform.position.y;
            transform.position = push;
            transform.position += (Vector3.Scale(distance.normalized * tension, Vector3.forward + Vector3.right)) * slippery;  //drag toward root, but not vertically
            transform.position += rayout.normal * slippery;
            
        }
        else
        {
            transform.position += Vector3.Scale(distance.normalized * tension, Vector3.forward + Vector3.right);  //drag toward root, but not vertically
        }
        //foreach (Vector3 i in cardinals)
        //{
        //    if (Physics.Raycast(transform.position, i, out rayout, Vector3.Scale(distance, i).magnitude))
        //    { transform.position = rayout.point - (i*(transform.localScale.y)); }
        //}

    }
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        if (other.gameObject.tag == "Player" && Input.GetButton("Fire3"))
        {
            UnHookObject();
            Debug.Log("Unhooked " + gameObject.name);
        }
        else if (other.gameObject.tag == "Hook" && gameObject.tag == "Hook")
        { 
            Vector3 cdistance = other.transform.position - transform.position;
            float distanceToPush = Mathf.Clamp((other.bounds.size.x * other.transform.localScale.x) - cdistance.magnitude, 0, (other.bounds.size.x * other.transform.localScale.x));
            //float distanceToPush = Mathf.Clamp((other.bounds.size.x) - cdistance.magnitude, 0, (other.bounds.size.x));
            transform.position += (cdistance.normalized * (0-distanceToPush));
        }
    }
}

