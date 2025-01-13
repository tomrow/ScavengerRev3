using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBumper : MonoBehaviour
{
    public float power;
    bool animate;
    int playSnd;
    float animCounter;
    public float counterSpeed;
    float animatorSize = 1; AudioSource jumpSoundControl;
    Transform animator, animator2;
    public bool angled;
    float platformPositionFullyExtended;


    // Start is called before the first frame update
    void Start()
    {
        if (counterSpeed == 0f) { counterSpeed = 8f; }
        animator = transform.Find("springModel").Find("Spring001");
        animator2 = transform.Find("springModel").Find("platform");
        jumpSoundControl = gameObject.GetComponent<AudioSource>();
        platformPositionFullyExtended = animator2.localPosition.y; //not really fully extended, but with an extension magnitude of 1

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.localScale = new Vector3(animatorSize, animatorSize, (0.3f + Mathf.Sin(Mathf.Deg2Rad * animCounter)));
        animator2.localPosition = new Vector3(0, (0.3f + Mathf.Sin(Mathf.Deg2Rad * animCounter)) * platformPositionFullyExtended, 0);
        if (animate)
        {
            animCounter+= counterSpeed;
            
            if (animCounter > 180)
            {
                animate = false;
                animCounter= 0f;
                animator.localScale = new Vector3(animatorSize, animatorSize, animatorSize);
            }

        }
        if (playSnd == 6) { jumpSoundControl.Play(); }
        if(playSnd>0) { playSnd--; }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerGroundProbe")
        {
            other.gameObject.transform.parent.parent.GetComponent<PlayerMovement>().vspeed = power;
            other.gameObject.transform.parent.parent.GetComponent<PlayerMovement>().speed2 *= 0.5f;
            other.gameObject.transform.parent.parent.position = transform.position;
            if (angled) { other.gameObject.transform.parent.parent.transform.localRotation = transform.localRotation; }
            other.gameObject.transform.parent.parent.transform.Translate(Vector3.up*(transform.lossyScale.y*1.1f));
            animate = true; animCounter = 0f; playSnd = 6;
        }
    }
}
