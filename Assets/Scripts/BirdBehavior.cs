using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    public static BirdBehavior Instance;

    public enum BirdState
    {
        Default,
        Warning,
        Dying,
        Dead
    }

    private BirdState birdState;

    [SerializeField] private float warningMinTimer;
    [SerializeField] private float warningMaxTimer;

    [SerializeField] private float dyingMinTimer;
    [SerializeField] private float dyingMaxTimer;

    private float currentTimer;
    private float timerGoal;

    private bool regainingOxygen;

    [SerializeField] private float regainOxygenTimerMin;
    [SerializeField] private float regainOxygenTimerMax;
    private float currentRegainOxygenTimer;
    private float regainOxygenTimerGoal;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        TryRegainOxygen();

        switch (birdState)
        {
            case BirdState.Default:

                break;
            case BirdState.Warning:

                currentTimer += Time.deltaTime;
                if (currentTimer >= timerGoal)
                {
                    currentTimer -= timerGoal;

                    int rand = Random.Range(1, 4);
                    AudioManager.Instance.Play($"BirdWarning{rand}", transform.position, gameObject, true);

                    timerGoal = Random.Range(warningMinTimer, warningMaxTimer);
                }

                break;
            case BirdState.Dying://todo: increase frequency when closer to dying

                currentTimer += Time.deltaTime;
                if (currentTimer >= timerGoal)
                {
                    currentTimer -= timerGoal;

                    int rand = Random.Range(1, 4);
                    AudioManager.Instance.Play($"BirdDying{rand}", transform.position, gameObject, true);

                    timerGoal = Random.Range(dyingMinTimer, dyingMaxTimer);
                }

                break;
            case BirdState.Dead:

                break;
        }
    }

    public void ChangeBirdState(BirdState state)
    {
        if (birdState == state || birdState == BirdState.Dead)
        {
            return;
        }
        birdState = state;

        switch (birdState)
        {
            case BirdState.Default:

                break;
            case BirdState.Warning:
                timerGoal = 0;
                break;
            case BirdState.Dying:
                timerGoal = 0;
                break;
            case BirdState.Dead:
                gameObject.SetActive(false);
                break;
        }
    }

    public void SwitchRegainOxygen(bool val)
    {
        if (regainingOxygen == val) return;

        if (val == true)
        {
            regainOxygenTimerGoal = Random.Range(regainOxygenTimerMin, regainOxygenTimerMax);
        }
        else
        {
            currentRegainOxygenTimer = 0;
        }

        regainingOxygen = val;
    }

    private void TryRegainOxygen()
    {
        if (regainingOxygen && birdState != BirdState.Default)
        {
            currentRegainOxygenTimer += Time.deltaTime;

            if (currentRegainOxygenTimer >= regainOxygenTimerGoal)
            {
                birdState = BirdState.Default;
                currentRegainOxygenTimer = 0;
            }
        }
    }
}
