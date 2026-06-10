using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaDrainRate;
    [SerializeField] private float staminaGainRate;
    public bool staminaPenalty;
    public bool staminaCooldown;
    private float staminaCooldownTimer;
    [SerializeField] private float staminaCooldownTimerGoal;


    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (PlayerInput.Instance.movementState == PlayerInput.MovementState.Sprinting)
        {
            DrainStamina();
        }
        else
        {
            GainStamina();
        }

        TryStaminaRecoverCooldownTimer();
    }

    private void DrainStamina()
    {
        if (currentStamina > 0)
        {
            currentStamina -= Time.deltaTime * staminaDrainRate;

            if (currentStamina <= 0)
            {
                PlayerInput.Instance.RequestChangeMovementState(PlayerInput.MovementState.Walking);
                staminaCooldown = true;
                staminaPenalty = true;
                PlayerInput.Instance.goalFOV = PlayerInput.Instance.drainedStaminaFOV;
                currentStamina = 0;
            }
        }
    }

    private void GainStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * staminaGainRate;

            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
            }
        }
    }

    private void TryStaminaRecoverCooldownTimer()
    {
        if (staminaCooldown)
        {
            staminaCooldownTimer += Time.deltaTime;

            if (staminaCooldownTimer >= staminaCooldownTimerGoal)
            {
                staminaCooldownTimer = 0;
                PlayerInput.Instance.goalFOV = PlayerInput.Instance.normalFOV;
                staminaCooldown = false;
            }
        }
    }
}
