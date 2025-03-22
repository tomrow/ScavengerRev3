using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpreadOut : MonoBehaviour
{
    public bool robotCannotUsePlatforms;
    public GameObject blob;
    robot r;
    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        r = GameObject.Find("robot").GetComponent<robot>();
        playerMovement = r.gameObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void HurtPlayer(PlayerMovement pm, GameObject gmo)
    {

        pm.beingAttackedByEnemy = true;
        pm.knockBackDir = (gmo.gameObject.transform.position - transform.position);
        pm.knockBackDir.y = 0;
        pm.knockBackDir = pm.knockBackDir.normalized;
        gmo.gameObject.transform.Find("Data").Find("Sound").Find("recvAttackSnd").gameObject.GetComponent<AudioSource>().Play();
        GameStateVariables.health -= (int)(GameStateVariables.maxHealth / 8f);//have to have a float somewhere in this otherwise it will try integer division and get wrong answer
    }
    private void OnTriggerStay(Collider other)
    {
        PlayerMovement opm = other.gameObject.GetComponent<PlayerMovement>();
        if (other.gameObject.tag == "CorruptionPlatform")
        {
            transform.position -= ((other.gameObject.transform.position - transform.position)) * Time.deltaTime;
        }
        else if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<robot>() != null && robotCannotUsePlatforms)
        {
            GameObject newBlob = Instantiate(blob, other.transform);
            newBlob.transform.position = other.ClosestPoint(transform.position);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player" && (playerMovement.stunTimer <= 0))
        { //pain

            if (playerMovement.invulnSeconds <= 0 && r.stunTimer <= 0.03f)
            { HurtPlayer(opm, other.gameObject); }
        }
    }
}
