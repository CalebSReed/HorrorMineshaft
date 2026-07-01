using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarbonMonoxide : MonoBehaviour
{
    [SerializeField] private float timerMin;
    [SerializeField] private float timerMax;
    [SerializeField] private float moveSpeed;
    private float currentTimerGoal;
    [SerializeField] private float nodeCheckRadius;
    private float currentTimer;

    private bool isMoving;
    private Vector3 destination;
    [SerializeField] LayerMask gasNodeLayer;

    private bool touchingPlayer;

    private GasNode currentNode;

    private void Start()
    {
        currentTimerGoal = Random.Range(timerMin, timerMax);
        CheckNearbyNodesForDestination();
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving)
        {
            currentTimer += Time.deltaTime;

            if (currentTimer >= currentTimerGoal)
            {
                if (touchingPlayer)
                {
                    Debug.Log("Player is inside, staying still for now");
                    currentTimer = 0;
                    currentTimerGoal = Random.Range(timerMin, timerMax);
                    return;
                }

                CheckNearbyNodesForDestination();
                isMoving = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, destination) <= .01f)
            {
                transform.position = destination;
                isMoving = false;
                currentTimer = 0;
                currentTimerGoal = Random.Range(timerMin, timerMax);
            }
        }
    }

    private void CheckNearbyNodesForDestination()
    {
        var nearbyNodes = Physics.SphereCastAll(transform.position, nodeCheckRadius, Vector3.up, 500, gasNodeLayer);

        if (currentNode != null)
        {
            currentNode.nodeTaken = false;
            currentNode = null;
        }

        while (currentNode == null)
        {
            int coinFlip = Random.Range(0, 2);
            if (coinFlip == 0)//choose random
            {
                Debug.Log("choosing random node");
                var rand = Random.Range(0, nearbyNodes.Length - 1);
                destination = nearbyNodes[rand].transform.position;
                currentNode = nearbyNodes[rand].transform.GetComponent<GasNode>();

                if (!currentNode.nodeTaken)
                {
                    currentNode.nodeTaken = true;
                }
                else
                {
                    currentNode = null;
                }
            }
            else//choose closest to player
            {
                Debug.Log("choosing node nearest to player");
                var newList = CalebUtils.SortListByDistance(nearbyNodes.ToList(), PlayerInput.Instance.transform);
                destination = newList[0].Item1.position;
                currentNode = newList[0].Item1.transform.GetComponent<GasNode>();

                if (!currentNode.nodeTaken)
                {
                    currentNode.nodeTaken = true;
                }
                else
                {
                    currentNode = null;
                }
            }
        }
        Debug.Log(destination);

    }

    private void OnTriggerEnter(Collider other)
    {
        var oxygenLevel = other.attachedRigidbody.GetComponent<OxygenLevel>();
        if (oxygenLevel)
        {
            touchingPlayer = true;
            oxygenLevel.BeginLosingOxygen();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var oxygenLevel = other.attachedRigidbody.GetComponent<OxygenLevel>();
        if (oxygenLevel)
        {
            touchingPlayer = false;
            oxygenLevel.EndLosingOxygen();
        }
    }
}
