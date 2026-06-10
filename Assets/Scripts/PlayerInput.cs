using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    public enum MovementState
    {
        Walking,
        Sprinting,
        Crouching
    }

    public MovementState movementState { get; private set; }

    public float moveSpeed;
    public float crouchSpeed;
    public float sprintSpeed;
    public bool disableControls;
    [SerializeField] private float maxInteractDistance;

    public Transform orientation;
    public Camera mainCam;
    [SerializeField] Transform cameraHolder;

    private float currentCameraHeight;
    private float cameraHeightGoal;
    [SerializeField] float cameraHeightAdjustSpeed;

    [SerializeField] private float crouchCameraHeight;
    [SerializeField] private float normalCameraHeight;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    [SerializeField] Collider playerCollider;

    public PlayerInputActions playerInput;

    [SerializeField] HealthManager healthManager;

    [SerializeField] private HoverText HoverText;
    private GameObject selectedObject;//object we are hovering with mouse

    [SerializeField] private Light playerLight;
    [SerializeField] private Light HUDPlayerLight;
    private float originalLightIntensity;
    private float originalHUDLightIntensity;
    private bool lightEnabled;

    [SerializeField] private Light nightLight;
    [SerializeField] private Light HUDNightLight;

    private float originalNightLightIntensity;
    [SerializeField] private float nightVisionTimerGoal;
    private float nightVisionTimer;
    private bool nightVisionActive;

    public float normalFOV;
    [SerializeField] private float sprintingFOV;
    public float drainedStaminaFOV;
    private float currentFOV;
    public float goalFOV;
    [SerializeField] private float fovAdjustSpeed;
    private StaminaManager staminaManager;

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerInput = new PlayerInputActions();
        playerInput.Enable();

        currentFOV = normalFOV;
        mainCam.fieldOfView = currentFOV;
        goalFOV = currentFOV;
        currentCameraHeight = cameraHolder.position.y;
        normalCameraHeight = cameraHolder.position.y;
        cameraHeightGoal = currentCameraHeight;

        //playerInput.PlayerDefault.InteractButton.performed += OnInteractButtonDown;
        //playerInput.PlayerDefault.ToggleLight.performed += ToggleLight;

        //playerInput.PlayerDefault.Crouch. += ToggleCrouch;

        //playerInput.PlayerDefault.MousePosition.Enable();
        //playerInput.PlayerDefault.InteractButton.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthManager.OnDeath += Die;
        lightEnabled = playerLight.gameObject.activeSelf;
        originalNightLightIntensity = nightLight.intensity;
        originalLightIntensity = playerLight.intensity;
        originalHUDLightIntensity = HUDPlayerLight.intensity;
        staminaManager = GetComponent<StaminaManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMovementInput();
        SelectObjects();
        CheckToActivateNightVision();
        TryActivateNightVision();
        TryActivateLight();
        MaintainFOV();
        CheckToEnterSprint();
        CheckToExitSprint();
        CheckToRemoveStaminaPenalty();
        MaintainCameraHeight();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetMovementInput()
    {
        horizontalInput = playerInput.PlayerDefault.Movement.ReadValue<Vector2>().x;
        verticalInput = playerInput.PlayerDefault.Movement.ReadValue<Vector2>().y;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0;

        if (movementState == MovementState.Walking)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        }
        else if (movementState == MovementState.Crouching)
        {
            rb.AddForce(moveDirection.normalized * crouchSpeed, ForceMode.Force);
        }
        else if (movementState == MovementState.Sprinting)
        {
            rb.AddForce(moveDirection.normalized * sprintSpeed, ForceMode.Force);
        } 
    }

    public void OnInteractButtonDown(InputAction.CallbackContext context)
    {
        if (context.performed && selectedObject != null)
        {
            selectedObject.GetComponent<Interactable>().OnInteract(null);
        }
    }

    public void ToggleSprint(InputAction.CallbackContext context)
    {
        if (context.performed && movementState != MovementState.Crouching && verticalInput > 0 && !staminaManager.staminaCooldown)
        {
            cameraHeightGoal = normalCameraHeight;
            movementState = MovementState.Sprinting;
            goalFOV = sprintingFOV;
        }
        //else if (context. && movementState != MovementState.Crouching && moveDirection.z > 0)
        else if (context.canceled && movementState != MovementState.Walking)
        {
            cameraHeightGoal = normalCameraHeight;
            movementState = MovementState.Walking;
            goalFOV = normalFOV;
        }
    }

    private void CheckToEnterSprint()
    {
        if (movementState != MovementState.Crouching && movementState != MovementState.Sprinting && verticalInput > 0 && playerInput.PlayerDefault.Sprint.ReadValue<float>() > 0 && !staminaManager.staminaPenalty && !staminaManager.staminaCooldown)
        {
            cameraHeightGoal = normalCameraHeight;
            movementState = MovementState.Sprinting;
            goalFOV = sprintingFOV;
        }
    }

    private void CheckToExitSprint()
    {
        if (movementState == MovementState.Sprinting && verticalInput <= 0)
        {
            cameraHeightGoal = normalCameraHeight;
            movementState = MovementState.Walking;
            goalFOV = normalFOV;
        }
    }

    public void ToggleCrouch(InputAction.CallbackContext context)
    {
        if (context.performed && movementState != MovementState.Sprinting)
        {
            cameraHeightGoal = crouchCameraHeight;
            movementState = MovementState.Crouching;
        }
        else if (context.canceled && movementState == MovementState.Crouching)
        {
            cameraHeightGoal = normalCameraHeight;
            movementState = MovementState.Walking;
        }
    }

    private void SelectObjects()
    {
        Ray ray = mainCam.ScreenPointToRay(playerInput.PlayerDefault.MousePosition.ReadValue<Vector2>());
        RaycastHit[] rayHitList = Physics.RaycastAll(ray, maxInteractDistance);

        var newList = CalebUtils.SortListByDistance(new List<RaycastHit>(rayHitList), transform);

        foreach (var rayHit in newList)
        {
            GameObject hitObj = rayHit.Item1.gameObject;
            if (hitObj.GetComponent<Interactable>() != null)
            {
                if (hitObj == selectedObject)
                {
                    return;
                }

                selectedObject = hitObj;
                HoverText.SetHoverText(hitObj.GetComponent<Interactable>().hoverText);
                return;
            }
        }
        selectedObject = null;
        HoverText.SetHoverText(string.Empty);
    }

    public void ToggleLight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lightEnabled = !lightEnabled;
            playerLight.gameObject.SetActive(lightEnabled);
            HUDPlayerLight.gameObject.SetActive(lightEnabled);

            playerLight.intensity = 0;
            HUDPlayerLight.intensity = 0;
            nightLight.intensity = 0;
            HUDNightLight.intensity = 0;

            if (lightEnabled)
            {
                nightLight.gameObject.SetActive(false);
                HUDNightLight.gameObject.SetActive(false);
                nightVisionActive = false;

                ColorAdjustments colorAdjustments;
                GlobalVolumeManager.Instance.globalVolume.profile.TryGet(out colorAdjustments);
                colorAdjustments.saturation.overrideState = false;
            }
            else
            {
                ColorAdjustments colorAdjustments;
                GlobalVolumeManager.Instance.globalVolume.profile.TryGet(out colorAdjustments);
                colorAdjustments.saturation.overrideState = true;
            }
        }
    }

    private void CheckToActivateNightVision()
    {
        if (!lightEnabled && !nightLight.gameObject.activeSelf)
        {
            nightVisionTimer += Time.deltaTime;

            if (nightVisionTimer >= nightVisionTimerGoal)
            {
                nightLight.gameObject.SetActive(true);
                HUDNightLight.gameObject.SetActive(true);

                //nightLight.intensity = 0;
                //HUDNightLight.intensity = 0;

                nightVisionActive = true;
            }
        }
        else if (lightEnabled)
        {
            nightVisionTimer = 0;
        }
    }

    private void TryActivateNightVision()
    {
        if (nightVisionActive)
        {
            if (nightLight.intensity < originalNightLightIntensity)
            {
                nightLight.intensity += Time.deltaTime / 2;
                HUDNightLight.intensity += Time.deltaTime / 2;
            }
            else
            {
                nightLight.intensity = originalNightLightIntensity;
                HUDNightLight.intensity = originalNightLightIntensity;
            }
        }
    }

    private void TryActivateLight()
    {
        if (lightEnabled && playerLight.intensity < originalLightIntensity)
        {
            playerLight.intensity += Time.deltaTime * originalLightIntensity * 2;
            HUDPlayerLight.intensity += Time.deltaTime * originalHUDLightIntensity * 2f;

            if (playerLight.intensity >= originalLightIntensity)
            {
                playerLight.intensity = originalLightIntensity;
                HUDPlayerLight.intensity = originalHUDLightIntensity;
            }
        }
        else if (lightEnabled)
        {
            playerLight.intensity = originalLightIntensity;
            HUDPlayerLight.intensity = originalHUDLightIntensity;
        }
    }

    private void MaintainFOV()
    {
        if (currentFOV != goalFOV)
        {
            currentFOV = Mathf.Lerp(currentFOV, goalFOV, Time.deltaTime * fovAdjustSpeed);
            mainCam.fieldOfView = currentFOV;
        }
    }

    private void MaintainCameraHeight()
    {
        if (currentCameraHeight != cameraHeightGoal)
        {
            currentCameraHeight = Mathf.Lerp(currentCameraHeight, cameraHeightGoal, Time.deltaTime *  cameraHeightAdjustSpeed);
            cameraHolder.position = new Vector3(cameraHolder.position.x, currentCameraHeight, cameraHolder.position.z);
        }
    }

    public void RequestChangeMovementState(MovementState state)
    {
        //if check if we need to ignore this call sometimes
        movementState = state;
    }

    private void CheckToRemoveStaminaPenalty()
    {
        if (staminaManager.staminaPenalty)
        {
            if (playerInput.PlayerDefault.Sprint.ReadValue<float>() == 0 || verticalInput <= 0)
            {
                staminaManager.staminaPenalty = false;
            }
        }
    }

    private void Die(object sender, System.EventArgs e)
    {
        healthManager.SetIsAlive(false);
        playerInput.Disable();
        CameraInput.DisableControls();
        Debug.Log("GAME OVER");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            healthManager.TakeDamage(999);
        }
    }
}
