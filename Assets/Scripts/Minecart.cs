using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour
{
    private Interactable interactable;
    [SerializeField] private SignBehavior signBehavior;
    [SerializeField] private int coalCounter;
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

            if (coalCounter > 0)
            {
                coalCounter--;
                signBehavior.SetSignText($"{coalCounter} Coal Deposits Left");
            }
            
            if (coalCounter <= 0)
            {
                signBehavior.SetSignText("YOU MUST LEAVE");
                var pos = transform.position;
                pos.y = 0;
                MonsterBehavior.Instance.SetAgentDestination(pos);
            } 
        }
    }
}
