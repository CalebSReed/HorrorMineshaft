using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalDeposit : MonoBehaviour
{
    [SerializeField] Interactable interactable;

    private void Start()
    {
        interactable.OnInteractEvent += GetMined;
    }

    private void GetMined(object sender, InteractArgs interactArgs)
    {
        Destroy(gameObject);
    }
}
