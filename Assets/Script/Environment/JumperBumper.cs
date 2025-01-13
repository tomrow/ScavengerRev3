using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperBumper : MonoBehaviour
{
    public float power;
    bool animate;
    int playSnd;
    float animCounter;
    public float counterSpeed;
    float animatorSize = 82; AudioSource jumpSoundControl;
    Transform animator;
    public bool angled;

    // Start is called before the first frame update
    void Start()
    {
        if (counterSpeed == 0f) { counterSpeed = 8f; }
        animator = transform.Find("hose");
        jumpSoundControl = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (animate)
        {
            animCounter+= counterSpeed;
            animator.localScale = new Vector3(animatorSize,animatorSize,animatorSize*(1+Mathf.Sin(Mathf.Deg2Rad * animCounter)));
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

            other.gameObject.transform.parent.parent.transform.Translate(Vector3.up*transform.lossyScale.y);
            if (angled) { other.gameObject.transform.parent.parent.transform.localRotation = transform.localRotation; }
            animate = true; animCounter = 0f; playSnd = 6;
        }
    }
}
