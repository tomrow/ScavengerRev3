using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;

public class AIBrain : MonoBehaviour
{

    #region **Variables**

    //the current set of AI actions
    UnityEvent currentAIDirective;
    [SerializeField, Tooltip("Default events for ai")]
    UnityEvent defaultActions;
    [SerializeField, Tooltip("events for ai when it is alerted")]
    UnityEvent alertActions;
    [SerializeField, Tooltip("events for ai when it is chasing a target")]
    UnityEvent huntActions;
    float pauseTimer = 0f;
    GameStateVariables player;
    #endregion

    void Start()
    {
        player = GameObject.FindObjectOfType<GameStateVariables>();
        currentAIDirective = defaultActions;
    }

    void Update()
    {
        if (updatePausedAi()) { return; }
        currentAIDirective.Invoke();

    }
    bool updatePausedAi()
    {
        if (pauseTimer > 0) { pauseTimer -= Time.deltaTime; pauseTimer = Mathf.Max(pauseTimer, 0); }
        return (pauseTimer > 0);
    }


    #region **AIState**
    public void SetStateDefault()
    { currentAIDirective = defaultActions; }
    public void SetStateHunt()
    { currentAIDirective = huntActions; }
    #endregion
    #region **AIEvents**
    public void EnemyJump(float force) { GetComponent<Rigidbody>()?.AddForce(Vector3.up * force); }
    public void AlertIfPlayerNearby(float radius) { if (DistanceToPlayer() < radius) { alertActions?.Invoke(); } }
    public void PauseAI(float milliseconds) { pauseTimer = milliseconds; }
    #endregion
    #region **Player chasing**
    float DistanceToPlayer() { return Vector3.Distance(transform.position, player.transform.position); }
    Vector3 CalculatePlayerPos(bool ignore_y = false)
    {
        Vector3 playerPos = player.gameObject.transform.position;
        playerPos.y = ignore_y ? transform.position.y : playerPos.y;
        return playerPos;
    }
    public void LookAtPlayer()
    {
        transform.LookAt(CalculatePlayerPos(true));
    }
    public void MoveTowardPlayer(float spd)
    {
        Vector3 playerPos = CalculatePlayerPos(true);
        //transform.LookAt(playerPos); //stare at player from different elevations?
        MoveTowardsPlayerUsingNavMesh(spd);
    }
    #endregion
    #region ***NavMesh***
    public void MoveTowardsPlayerUsingNavMesh(float spd)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) { rb.isKinematic = true; }
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent) { agent.SetDestination(player.transform.position); }
        agent.speed = spd;
    }
    #endregion
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttackBox") 
        {//die
            
        }
    }
}
