using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    private Transform childCam;
    private CharacterController characterController;
    private IA_Camera defaultInput;

    [Header("Movement")]
    [SerializeField] private float camDefaultMovementSpeed;
    [SerializeField] private float camFastMovementSpeed;
    [SerializeField] private float camSlowMovementSpeed;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float posYMax;
    [SerializeField] private float posYMin;
    [SerializeField] private bool speedHold;
    [SerializeField] private bool slowHold;
    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;
    private Vector3 input_Movement;
    private float curCamMovementSpeed;
    private bool isFast;
    private bool isSlow;

    [Header("View")]
    [SerializeField] private float viewXSensitivity;
    [SerializeField] private float viewYSensitivity;
    [SerializeField] private float viewYMax;
    [SerializeField] private float viewYMin;
    [SerializeField] private bool viewXInvert;
    [SerializeField] private bool viewYInvert;
    private Vector2 input_View;
    private Vector3 newCamRot;
    private Vector3 newCharRot;    
    
    private void Awake()
    {
        childCam = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        defaultInput = new IA_Camera();

        defaultInput.CameraMovement.Move.performed += e => input_Movement = e.ReadValue<Vector3>();
        defaultInput.CameraMovement.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.CameraMovement.SpeedUp.performed += e => ToggleSpeedUp();
        defaultInput.CameraMovement.SpeedUpReleased.performed += e => HoldSpeedUp();
        defaultInput.CameraMovement.SlowDown.performed += e => ToggleSlowDown();
        defaultInput.CameraMovement.SlowDownReleased.performed += e => HoldSlowDown();

        defaultInput.Enable();

        curCamMovementSpeed = camDefaultMovementSpeed;

        newCharRot = transform.localRotation.eulerAngles;
        newCamRot = childCam.localRotation.eulerAngles;
    }

    void Update()
    {
        CalculateView();
        CalculateMovement();
    }

    private void CalculateView()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Confined;

            newCharRot.y += viewXSensitivity * (viewXInvert ? -input_View.x : input_View.x) * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(newCharRot);

            newCamRot.x += viewYSensitivity * (viewYInvert ? input_View.y : -input_View.y) * Time.deltaTime;

            newCamRot.x = Mathf.Clamp(newCamRot.x, viewYMin, viewYMax);

            childCam.localRotation = Quaternion.Euler(newCamRot);
        }

        if (Input.GetMouseButtonUp(1))
            Cursor.lockState = CursorLockMode.None;
    }

    private void CalculateMovement()
    {
        if (input_Movement == new Vector3(0, 0, 0) && input_Movement == new Vector3(0, 0, 0))
        {
            isFast = false;
            isSlow = false;
        }

        if (isFast)
            curCamMovementSpeed = camFastMovementSpeed;

        if (isSlow)
            curCamMovementSpeed = camSlowMovementSpeed;

        if (!isFast && !isSlow)
            curCamMovementSpeed = camDefaultMovementSpeed;

        newMovementSpeed = Vector3.SmoothDamp(
            newMovementSpeed,
            new Vector3(curCamMovementSpeed * input_Movement.x * Time.deltaTime, curCamMovementSpeed * input_Movement.z * Time.deltaTime, curCamMovementSpeed * input_Movement.y * Time.deltaTime),
            ref newMovementSpeedVelocity,
            smoothSpeed
            );

        var moveToLocal = transform.TransformDirection(newMovementSpeed);
        characterController.Move(moveToLocal);

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, posYMin, posYMax);
        transform.position = pos;
    }

    private void ToggleSpeedUp()
    {
        if (input_Movement == new Vector3(0, 0, 0) && input_Movement == new Vector3(0, 0, 0))
        {
            isFast = false;
            return;
        }

        isFast = !isFast;
    }

    private void HoldSpeedUp()
    {
        if (speedHold)
            isFast = false;
    }

    private void ToggleSlowDown()
    {
        if (input_Movement == new Vector3(0, 0, 0) && input_Movement == new Vector3(0, 0, 0))
        {
            isSlow = false;
            return;
        }

        isSlow = !isSlow;
    }

    private void HoldSlowDown()
    {
        if (slowHold)
            isSlow = false;
    }
}
