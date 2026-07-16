using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textRenderer;
    private int dialogueCounter;
    public List<string> dialogueList = new List<string>();
    public bool intermission;

    private void Start()
    {
        textRenderer.text = dialogueList[dialogueCounter];
    }

    public void ProgressDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dialogueCounter++;

            if (dialogueCounter >= dialogueList.Count)
            {
                if (intermission)
                {
                    Application.Quit();
                    return;
                }
                SceneManager.LoadScene(1);
            }

            textRenderer.text = dialogueList[dialogueCounter];
        }
    }
}
