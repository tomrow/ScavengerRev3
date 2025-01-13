using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldCamera : MonoBehaviour
{
    public bool enableUpdates;
    // Start is called before the first frame update
    public RenderTexture cubemap;
    //public RenderTexture cubemapSmall;
    int[] masks = { 0b00000011, 0b00001100, 0b00110000 };
    int currentMask;
    int wait;
    Camera cmcamera;
    void Start()
    {
        cmcamera = GetComponent<Camera>();
        cmcamera.RenderToCubemap(cubemap);
    }

    // Update is called once per frame
    void UpdateOldCubeMap()
    {
        //currentMask = currentMask % 3;
        //cmcamera.RenderToCubemap(cubemap, masks[currentMask]);
        
        //cmcamera.RenderToCubemap(cubemapSmall, 1 << currentMask);
        wait++;
        if (wait > 40)
        {
            currentMask = currentMask % 6;
            cmcamera.RenderToCubemap(cubemap, 1 << currentMask);
            wait = 0;
            currentMask++;
        }
    }
    private void FixedUpdate()
    {
        if (enableUpdates) { UpdateOldCubeMap(); }
    }
}
