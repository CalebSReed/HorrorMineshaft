using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingManager : MonoBehaviour
{
    public void HearSound(Vector3 soundPos)
    {
        GetComponent<MonsterBehavior>().SetAgentDestination(soundPos);
    }
}
