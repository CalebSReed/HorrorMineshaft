using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshPro textRenderer;

    private void Start()
    {
        textRenderer.text = "5 Coal Deposits Left";
    }

    public void SetSignText(string val)
    {
        textRenderer.text = val;
    }
}
