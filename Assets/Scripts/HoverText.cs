using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoverText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hoverTextUI;
    private void Awake()
    {
        SetHoverText(string.Empty);
    }

    public void SetHoverText(string text)
    {
        hoverTextUI.text = text;
    }
}
