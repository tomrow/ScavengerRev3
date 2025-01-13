using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviour
{
    [SerializeField] GameObject _item;
    [SerializeField] Vector3 _offset = new Vector3(1, 0, 0);
    float approxSize;
    public AudioSource beep;
    private void Start()
    {
        beep = GetComponent<AudioSource>();
    }
    public void SpawnObject()
    {
        approxSize = transform.lossyScale.x + transform.lossyScale.z;
        approxSize = (float)approxSize/2;
        Instantiate(_item, transform.position + (_offset * approxSize), transform.rotation, transform.parent);
        beep.Play();
    }
}

