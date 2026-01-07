using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CableProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    public LauncherPointer sourceLauncher;
    public float range;
    float reach;
    Transform player;
    float gravityVelocity;
    RaycastHit rayout;
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        gravityVelocity = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out rayout, transform.localScale.y / 2f))
        {
            gravityVelocity = 0;
            transform.position = rayout.point + (Vector3.up * transform.localScale.y * 0.45f);
            if (rayout.transform.tag == "Respawn")
            { Destroy(this); }
        }
        else
        {
            gravityVelocity += Time.fixedDeltaTime / 4;
            transform.position += (Vector3.down * gravityVelocity);
        }
        transform.position = player.position + (transform.forward * reach);
        reach += Time.fixedDeltaTime * 2;
        if (reach > range) { Destroy(this); }
    }
    private void OnCollisionEnter(Collision collision)
    {
        HookedItem hookable = collision.gameObject.GetComponent<HookedItem>();
        if (hookable != null) { sourceLauncher.GenerateCableSegments(hookable); }
        Destroy(this);

    }
}
