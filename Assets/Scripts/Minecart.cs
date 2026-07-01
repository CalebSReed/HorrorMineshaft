using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour
{
    private Interactable interactable;
    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnInteractEvent += DepositCoal;
    }

    private void DepositCoal(object sender, InteractArgs e)
    {
        if (PlayerInput.Instance.carryingCoal)
        {
            PlayerInput.Instance.DepositCoal();
        }
    }
}
