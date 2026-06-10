using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    public event EventHandler<InteractArgs> OnInteractEvent;
    public string hoverText;
    [SerializeField] private string hoverActionText;

    private void Start()
    {
        SetHoverText(hoverActionText);
    }

    public void OnInteract(InteractArgs args)
    {
        OnInteractEvent?.Invoke(this, args);
    }

    public void SetHoverText(string text)
    {
        hoverText = $"{PlayerInput.Instance.playerInput.PlayerDefault.InteractButton.GetBindingDisplayString(0)}: {text}";
    }
}
