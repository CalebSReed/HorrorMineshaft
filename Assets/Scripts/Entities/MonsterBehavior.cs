using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class MonsterBehavior : MonoBehaviour
{
    public static MonsterBehavior Instance { get; private set; }

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float aiTickUpdateLength;
    private WaitForSeconds aiUpdateTimer;
    [SerializeField] private float maxSightRange;
    [SerializeField] private float maxAngleRange;
    private bool playerIsSeen;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float minWanderWait;
    [SerializeField] private float maxWanderWait;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float chaseSpeed;
    private bool waitingToWander;
    private float waitingTimer;
    private float waitingGoal;
    private bool reachedDestination = true;
    [SerializeField] private float wanderRange;
    private bool isChasing;
    [SerializeField] private Animator anim;

    [SerializeField] private float walkingAcceleration;
    [SerializeField] private float chasingAcceleration;

    private bool playerSpottedInCubbyHole;

    public enum MonsterState
    {
        Default,
        Chasing,
        Wandering,
        Investigating,
        Waiting
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        agent.acceleration = walkingAcceleration;
        agent.SetDestination(transform.position);
        aiUpdateTimer = new WaitForSeconds(aiTickUpdateLength);
        StartCoroutine(TryToChase());
        StartCoroutine(CheckForPlayer());
        StartCoroutine(CheckIfDestinationReached());
        StartCoroutine(WaitToTalk());
    }

    private void Update()
    {
        Wander();
    }

    private IEnumerator WaitToTalk()
    {
        var rand = Random.Range(4f, 10f);
        yield return new WaitForSeconds(rand);
        var audioRand = Random.Range(1, 4);
        AudioManager.Instance.Play($"MonsterTalk{audioRand}", transform.position, gameObject, false, true, false);
        StartCoroutine(WaitToTalk());
    }

    private IEnumerator CheckIfDestinationReached()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, agent.destination) <= 1f)
            {
                reachedDestination = true;
                anim.SetBool("isMoving", false);

                if (!playerIsSeen)
                {
                    isChasing = false;
                    agent.acceleration = walkingAcceleration;
                    SetMoveSpeed(walkSpeed);
                    anim.SetBool("isChasing", false);
                }
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
                    anim.SetBool("isMoving", true);
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
                    var checkPos = new Vector3(transform.position.x, PlayerInput.Instance.transform.position.y + .25f, transform.position.z);
                    Physics.Raycast(checkPos, monsterToPlayerDir, out rayHit, maxSightRange);
                    //if (rayHit.collider && rayHit.collider.attachedRigidbody) Debug.Log(rayHit.collider.attachedRigidbody.gameObject.name);

                    if (rayHit.collider && rayHit.collider.attachedRigidbody && rayHit.collider.attachedRigidbody.gameObject.CompareTag("Player"))
                    {
                        playerIsSeen = true;
                        isChasing = true;
                        agent.acceleration = chasingAcceleration;
                        anim.SetBool("isChasing", true);
                        SetMoveSpeed(chaseSpeed);
                        //Debug.Log("seen");
                        continue;
                    } 
                }
            }

            Debug.Log($"unseen + {playerIsSeen}");

            if (playerIsSeen && PlayerInput.Instance.inCubbyHole)
            {
                playerSpottedInCubbyHole = true;
                print("in cubby");
            }
            else if (playerIsSeen && !PlayerInput.Instance.inCubbyHole)
            {
                playerSpottedInCubbyHole = false;
                print("out cubby");
            }

            playerIsSeen = false;    
        }
    }

    private IEnumerator TryToChase()//Acceleration controls how easily monster turns, when wandering it shouldnt turn so quickly but when close to the player chasing it down, it should snap towards player, so dynamically change acceleration pls!
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

    public void SetAgentDestination(Vector3 pos)//todo swap navmesh surfaces when monster enters crawling mode to crawl through tight spaces
    {
        //Debug.Log("begin listening");
        if (!playerIsSeen)
        {
            reachedDestination = false;
            agent.destination = pos;
            isChasing = true;
            agent.acceleration = chasingAcceleration;
            anim.SetBool("isChasing", true);
            SetMoveSpeed(chaseSpeed);
            //Debug.Log("goingg now");
        }
    }

    private void SetMoveSpeed(float val)
    {
        agent.speed = val;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CubbyHole") && playerSpottedInCubbyHole)
        {
            PlayerInput.Instance.GetComponent<HealthManager>().TakeDamage(999);
        }
    }
}
