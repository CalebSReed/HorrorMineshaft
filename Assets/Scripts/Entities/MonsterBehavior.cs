using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class MonsterBehavior : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float aiTickUpdateLength;
    private WaitForSeconds aiUpdateTimer;
    [SerializeField] private float maxSightRange;
    [SerializeField] private float maxAngleRange;
    private bool playerIsSeen;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float minWanderWait;
    [SerializeField] private float maxWanderWait;
    private bool waitingToWander;
    private float waitingTimer;
    private float waitingGoal;
    private bool reachedDestination = true;
    [SerializeField] private float wanderRange;

    public enum MonsterState
    {
        Default,
        Chasing,
        Wandering,
        Investigating,
        Waiting
    }


    private void Start()
    {
        agent.SetDestination(transform.position);
        aiUpdateTimer = new WaitForSeconds(aiTickUpdateLength);
        StartCoroutine(TryToChase());
        StartCoroutine(CheckForPlayer());
        StartCoroutine(CheckIfDestinationReached());
    }

    private void Update()
    {
        Wander();
    }

    private IEnumerator CheckIfDestinationReached()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, agent.destination) <= 1f)
            {
                reachedDestination = true;
            }
            else
            {
                //Debug.Log(agent.destination);
                reachedDestination = false;
            }
            yield return aiUpdateTimer;
        }
    }

    private void Wander()
    {
        if (reachedDestination)
        {
            if (waitingToWander)
            {
                waitingTimer += Time.deltaTime;

                if (waitingTimer >= waitingGoal)
                {
                    waitingToWander = false;
                    waitingTimer = 0;
                    reachedDestination = false;
                    SetNewWanderDestination();
                }
            }
            else
            {
                waitingToWander = true;
                waitingGoal = Random.Range(minWanderWait, maxWanderWait);
            }
        }
    }

    private IEnumerator CheckForPlayer()
    {
        while (true)
        {
            yield return aiUpdateTimer;
            if (Vector3.Distance(PlayerInput.Instance.transform.position, transform.position) < maxSightRange)
            {
                var monsterToPlayerDir = (new Vector3(PlayerInput.Instance.transform.position.x, transform.position.y, PlayerInput.Instance.transform.position.z) - transform.position).normalized;
                var playerToMonsterAngle = Vector3.Angle(transform.forward, monsterToPlayerDir);
                //Debug.Log(playerToMonsterAngle);
                if (playerToMonsterAngle < maxAngleRange)
                {
                    RaycastHit rayHit;
                    var checkPos = new Vector3(transform.position.x, PlayerInput.Instance.transform.position.y + 1f, transform.position.z);
                    Physics.Raycast(checkPos, monsterToPlayerDir, out rayHit, maxSightRange);
                    //if (rayHit.collider && rayHit.collider.attachedRigidbody) Debug.Log(rayHit.collider.attachedRigidbody.gameObject.name);

                    if (rayHit.collider && rayHit.collider.attachedRigidbody && rayHit.collider.attachedRigidbody.gameObject.CompareTag("Player"))
                    {
                        playerIsSeen = true;
                        //Debug.Log("seen");
                        continue;
                    } 
                }
            }
            //Debug.Log("unseen");
            playerIsSeen = false;    
        }
    }

    private IEnumerator TryToChase()
    {
        while (true)
        {
            if (playerIsSeen)
            {
                agent.SetDestination(PlayerInput.Instance.transform.position);
            }
            yield return aiUpdateTimer;
        }
    }

    private void SetNewWanderDestination()
    {
        var hits = Physics.OverlapSphere(transform.position, wanderRange);
        var destinationList = new List<Vector3>();

        foreach (var hit in hits)
        {         
            if (hit.CompareTag("WanderNode"))
            {
                destinationList.Add(hit.transform.position);
            }
        }
        //Debug.Log(destinationList.Count);
        var rand = Random.Range(0, destinationList.Count);
        agent.SetDestination(destinationList[rand]);
    }

    public void SetAgentDestination(Vector3 pos)//todo add diff steps with diff radiuses when crouching and running etc
    {
        Debug.Log("begin listening");
        if (!playerIsSeen)
        {
            reachedDestination = false;
            agent.destination = pos;
            Debug.Log("goingg now");
        }
    }
}
