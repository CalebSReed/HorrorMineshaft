using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingManager : MonoBehaviour
{
    [SerializeField] private float alertThreshold;//add alert progress when hearing a sound, some sounds will alert more, when reach threshold, investigate location
    private float alertProgress;

    [SerializeField] private float relaxTimerGoal;
    private float relaxTimer;
    private bool runningTimer;
    private bool relaxed;

    public void HearSound(Vector3 soundPos, float alertVal)
    {
        runningTimer = true;
        relaxed = false;
        relaxTimer = 0f;
        alertProgress += alertVal;

        if (alertProgress >= alertThreshold)
        {
            GetComponent<MonsterBehavior>().SetAgentDestination(soundPos);
            alertProgress = alertThreshold;
        }
    }

    private void Update()
    {
        if (runningTimer)
        {
            relaxTimer += Time.deltaTime;

            if (relaxTimer >= relaxTimerGoal)
            {
                relaxed = true;
                relaxTimer = 0f;
                runningTimer = false;
            }
        }
        else if (relaxed && alertProgress > 0)
        {
            alertProgress -= Time.deltaTime * 10;

            if (alertProgress < 0)
            {
                alertProgress = 0;
            }
        }
    }
}
