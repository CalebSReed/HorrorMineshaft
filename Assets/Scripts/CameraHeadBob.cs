using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    public float bobIntensity;
    public float bobIntensityX;
    public float bobSpeed;
    public float easingCoefficient;
    public float speedMult;

    private Vector3 OriginalOffset;
    private float SinTime;
    private float stepTimer;
    private Vector3 lastPlayerPosition;
    private bool playerIsMoving;

    // Start is called before the first frame update
    void Start()
    {
        lastPlayerPosition = PlayerInput.Instance.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = new Vector3(PlayerInput.Instance.playerInput.PlayerDefault.Movement.ReadValue<Vector2>().x, PlayerInput.Instance.playerInput.PlayerDefault.Movement.ReadValue<Vector2>().y);
        if (inputVector.magnitude > 0f)
        {
            SinTime += Time.deltaTime * bobSpeed * speedMult;
            stepTimer += Time.deltaTime * bobSpeed * speedMult;

            if (stepTimer >= Mathf.PI)
            {
                var rand = Random.Range(1, 6);
                if (PlayerInput.Instance.movementState != PlayerInput.MovementState.Crouching && PlayerInput.Instance.movementState != PlayerInput.MovementState.CrouchingDrained)
                {
                    AudioManager.Instance.Play($"PlayerStep{rand}", transform.position, PlayerInput.Instance.gameObject, true);
                }
                stepTimer -= Mathf.PI;
            }
        }
        else
        {
            SinTime = 0f;
            stepTimer = 0f;
            transform.localPosition = Vector3.Slerp(transform.localPosition, Vector3.zero, Time.deltaTime * easingCoefficient);
            return;
        }

        float sinAmountY = -Mathf.Abs(bobIntensity * Mathf.Sin(SinTime));
        Vector3 sinAmountX = transform.right * bobIntensity * Mathf.Cos(SinTime) * bobIntensityX;

        var Offset = new Vector3
        {
            x = OriginalOffset.x,
            y = OriginalOffset.y + sinAmountY + .1f,
            z = OriginalOffset.z
        };

        Offset += sinAmountX;

        var targetPos = Vector3.zero + Offset;
        transform.localPosition = Vector3.Slerp(transform.localPosition, targetPos, Time.deltaTime * easingCoefficient);
    }

    private void CheckPlayerMovement()
    {
        if (Vector3.Distance(lastPlayerPosition, PlayerInput.Instance.transform.position) > 0f)
        {
            playerIsMoving = true;
        }
        else
        {
            playerIsMoving = false;
        }

        lastPlayerPosition = PlayerInput.Instance.transform.position;
    }
}