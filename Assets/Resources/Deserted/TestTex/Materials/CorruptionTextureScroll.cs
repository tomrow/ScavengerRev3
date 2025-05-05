using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionTextureScroll : MonoBehaviour
{
    MeshRenderer mr;
    Material mat;
    public Vector2 scrollDir;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mat.mainTextureOffset += scrollDir * Time.fixedDeltaTime;
        while (mat.mainTextureOffset.x > 1)
        { mat.mainTextureOffset += Vector2.left; }
        while (mat.mainTextureOffset.x < 0)
        { mat.mainTextureOffset += Vector2.right; }
        while (mat.mainTextureOffset.y > 1)
        { mat.mainTextureOffset += Vector2.down; }
        while (mat.mainTextureOffset.y < 0)
        { mat.mainTextureOffset += Vector2.up; }
    }
}
