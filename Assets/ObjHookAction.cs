using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHookAction : MonoBehaviour
{
    [SerializeField] GameObject parent;
    CableConnector connector;
    // Start is called before the first frame update
    void Start()
    {
        connector = GetComponent<CableConnector>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        HookedItem item = other.gameObject.GetComponent<HookedItem>();
        //if the item is:
        //1. not null
        //2. not the root of the splitter
        //3. the hook has not hooked something already
        //4. the touched item has not been hooked already
        //then hook it
        if (item != null && connector.hooked == null && other.gameObject != parent && item.towParent == null) {connector.HookObject(item); }
    }
}
