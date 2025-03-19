using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class blobs : MonoBehaviour
{
    float speed;
    Vector3 lookup;
    GameObject robotGO;
    PlayerMovement robot;
    Transform sphere;
    MeshRenderer mesh;
    ParticleSystem _particleSystem;
    ParticleSystem.EmissionModule _particleSystemEmissionModule;
    LineRenderer lineRenderer;
    public bool AttackableOnlyWhenStunned;
    public bool AbsorbableWhenStunned;
    Vector3 LerpToCharacterStartPos;
    Transform LerpTarget;
    bool lerping;
    float lerpPercent;
    // Start is called before the first frame update
    void Start()
    {
        //speed = (10 * Random.value - 5);
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystemEmissionModule = _particleSystem.emission;
        sphere = transform.GetChild(0);
        mesh = sphere.gameObject.GetComponent<MeshRenderer>();
        speed = 5;
        lookup = new Vector3 ((2 * Random.value - 1), (2 * Random.value - 1), (2 * Random.value - 1)).normalized;
        robotGO = transform.parent.gameObject;
        robot = robotGO.GetComponent<PlayerMovement>();
        lineRenderer = GetComponent<LineRenderer>();
        lerpPercent = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        mesh.enabled = robot.stunTimer <= 0.03f;
        lineRenderer.enabled = robot.stunTimer <= 0.03f;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.parent != null ? transform.parent.position: transform.position);
        _particleSystemEmissionModule.enabled = robot.stunTimer >= 0.03f; //sparks instead of sphere
        if (robot.stunTimer <= 0.03f && !lerping)
        {
            transform.parent = robotGO.transform;
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            transform.Translate(Vector3.forward * -0.1f * Time.deltaTime); //orbit robot
            transform.LookAt(transform.parent.position, lookup);
            float wideness = GameObject.Find("characterMulti").GetComponent<characterMulti>().blobWideness;
            if (transform.localPosition.magnitude > wideness) { transform.Translate(Vector3.forward * 2f * Time.deltaTime); }
            if (transform.localPosition.magnitude > wideness*2.5f) { Destroy(gameObject); }
        }
        else 
        {
            transform.parent = null; //stop moving and dont track robot until the robots un-stunned
            if (lerping)
            { 
                lerpPercent += Time.deltaTime *2f;
                transform.position = Vector3.Lerp(LerpToCharacterStartPos, LerpTarget.position, (Mathf.Tan(lerpPercent) / 1.55f) * (Mathf.Tan(lerpPercent) / 1.55f));
                if (lerpPercent >= 1) 
                {
                    GameStateVariables.score += 1;
                    Destroy(gameObject);
                }
            }
        }
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

    private void KillBlobFromAttack(Transform target)
    {
        LerpToCharacterStartPos = transform.position;
        LerpTarget = target;
        lerping = true;
        transform.parent = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy");
        

        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<robot>() == null)
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if (AbsorbableWhenStunned && transform.parent == null)
            {
                KillBlobFromAttack(other.transform);
            }
            else if (playerMovement.attacking && AttackableOnlyWhenStunned && transform.parent == null)
            {
                KillBlobFromAttack(other.transform);
            }
            else if (playerMovement.attacking && AttackableOnlyWhenStunned && transform.parent != null)
            {
                if (playerMovement.invulnSeconds <= 0 && robot.stunTimer <= 0.03f)
                { HurtPlayer(playerMovement, other.gameObject); }
            }
            else if (playerMovement.attacking && !AttackableOnlyWhenStunned )
            {
                KillBlobFromAttack(other.transform);
            }
            else
            {
                if (playerMovement.invulnSeconds <= 0 && robot.stunTimer <= 0.03f)
                { HurtPlayer(playerMovement, other.gameObject); }

            }
        }
    }
}
