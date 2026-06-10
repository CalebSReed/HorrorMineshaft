using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform cameraPos;

    public float sensX;
    public float sensY;

    public float xRotation;
    public float yRotation;

    public float mouseX;
    public float mouseY;

    private bool disableControls;

    public static CameraInput Instance { get; private set; }
    [SerializeField] private Transform cameraHolder;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        EnableControls();
    }

    // Update is called once per frame
    private void Update()
    {
        ReadCursorInput();
    }

    private void LateUpdate()
    {
        DoMovement();
    }

    public static void DisableControls()
    {
        Instance.disableControls = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void EnableControls()
    {
        Instance.disableControls = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ReadCursorInput()
    {
        if (disableControls)
        {
            return;
        }
        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    private void DoMovement()
    {
        if (disableControls)
        {
            return;
        }

        cameraHolder.position = cameraPos.position;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
