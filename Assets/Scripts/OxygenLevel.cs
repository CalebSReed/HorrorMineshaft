using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenLevel : MonoBehaviour
{
    [field: SerializeField] public float maxOxygen { get; private set; }
    [field: SerializeField] public float warningLevel { get; private set; }
    [field: SerializeField] public float dyingLevel { get; private set; }
    [field: SerializeField] public float birdDeathLevel { get; private set; }
    [field: SerializeField] public float currentOxygen {  get; private set; }
    [field: SerializeField] public bool losingOxygen { get; private set; }

    private void Start()
    {
        currentOxygen = maxOxygen;
    }

    private void Update()
    {
        if (losingOxygen)
        {
            LoseOxygen();
        }
        else if (currentOxygen < maxOxygen)
        {
            RegainOxygen();
        }
    }

    public void BeginLosingOxygen()
    {
        losingOxygen = true;
    }
    
    public void EndLosingOxygen()
    {
        losingOxygen = false;
    }

    private void LoseOxygen()
    {
        BirdBehavior.Instance.SwitchRegainOxygen(false);

        if (currentOxygen <= 0)
        {
            currentOxygen = 0;
            GetComponent<HealthManager>().TakeDamage(99);
        }
        else
        {
            currentOxygen -= Time.deltaTime;

            if (currentOxygen <= birdDeathLevel)
            {
                BirdBehavior.Instance.ChangeBirdState(BirdBehavior.BirdState.Dead);
            }
            else if (currentOxygen <= dyingLevel)
            {
                BirdBehavior.Instance.ChangeBirdState(BirdBehavior.BirdState.Dying);
            }
            else if (currentOxygen <= warningLevel)
            {
                BirdBehavior.Instance.ChangeBirdState(BirdBehavior.BirdState.Warning);
            }
        }
    }

    private void RegainOxygen()
    {
        currentOxygen += Time.deltaTime;
        
        if (currentOxygen > maxOxygen)
        {
            currentOxygen = maxOxygen;
        }

        BirdBehavior.Instance.SwitchRegainOxygen(true);

        /*if (currentOxygen > warningLevel)
        {
            BirdBehavior.Instance.ChangeBirdState(BirdBehavior.BirdState.Default);
        }
        else if (currentOxygen > dyingLevel)
        {
            BirdBehavior.Instance.ChangeBirdState(BirdBehavior.BirdState.Warning);
        }*/
    }
}
