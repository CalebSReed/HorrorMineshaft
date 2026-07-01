using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalDeposit : MonoBehaviour
{
    [SerializeField] Interactable interactable;

    [SerializeField] private Mesh phase2;
    [SerializeField] private Mesh phase3;

    private bool beingMined;

    [SerializeField] private float maxMineTime;
    private float currentMineTime;

    private void Start()
    {
        currentMineTime = maxMineTime;
        interactable.OnInteractEvent += GetMined;
    }

    private void Update()
    {
        if (beingMined)
        {
            if (currentMineTime <= 5)
            {
                GetComponent<MeshFilter>().mesh = phase3;
            }
            else if (currentMineTime <= 10)
            {
                GetComponent<MeshFilter>().mesh = phase2;
            }

            if (!PlayerInput.Instance.isMining)
            {
                beingMined = false;
            }

            currentMineTime -= Time.deltaTime;

            if (currentMineTime <= 0)
            {
                PlayerInput.Instance.CarryCoal();
                PlayerInput.Instance.StopMining();
                Destroy(gameObject);
            }
        }
    }

    private void GetMined(object sender, InteractArgs interactArgs)
    {
        if (!PlayerInput.Instance.carryingCoal)
        {
            PlayerInput.Instance.StartMining();
            beingMined = true;
        }
    }
}
