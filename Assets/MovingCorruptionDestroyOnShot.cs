using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCorruptionDestroyOnShot : MonoBehaviour
{
    public GameObject growth;
    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<shotgun>() != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            for (int i = 0; i < 10; i++)
            {
                Instantiate(
                    growth,
                    transform.position + new Vector3(
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f)
                        ), 
                    Quaternion.identity
                    );
            }
            Destroy(gameObject); 
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<shotgun>() != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            for (int i = 0; i < 10; i++)
            {
                Instantiate(
                    growth,
                    transform.position + new Vector3(
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f)
                        ),
                    Quaternion.identity
                    );
            }
            Destroy(gameObject);
        }
    }
}
