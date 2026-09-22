
using System;
using System.Drawing.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;
public class MouseLook : MonoBehaviour
{
    public InputActionAsset InputActions;
    public CharacterController playerCharacterController;
    public PhysicsRaycaster playerRaycaster;

    private Vector2 playerMoveAmount;

    private InputAction playerMoveAction;
    private InputAction playerJumpAction;

    private InputAction playerLookAction;
    private InputAction playerSprintAction;
    private InputAction playerInteractAction;
    private RaycastHit playerLooked;
    private InputAction playerTypeAction;
    private InputAction playerCancelAction;
    public PanelRenderer playerUI;


    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float playerWalkSpeed=5.0f;
    [SerializeField]
    private bool SmoothCamera=false;
    [SerializeField]
    private float playerRotateDampening=5f;
    [SerializeField]
    private float sprintSpeed=4;

    private float verticalVelocity = 0f;
    private float gravity = -9.8f;
    private float jumpHeight =5.0f;
    private Quaternion lookTarget;
    private Vector2 lookAngle;
    private bool looking;
    private Terminal termlooked;
    private bool inMonitor;
    private IDisposable unsub;
    

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
        InputActions.FindActionMap("UI").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
        InputActions.FindActionMap("UI").Disable();
    }

    private void Awake()
    {
        playerMoveAction = InputSystem.actions.FindAction("Move");
        playerJumpAction = InputSystem.actions.FindAction("Jump");
        playerLookAction = InputSystem.actions.FindAction("Look");
        playerSprintAction = InputSystem.actions.FindAction("Sprint");
        playerInteractAction = InputSystem.actions.FindAction("Interact");
        playerCancelAction = InputSystem.actions.FindAction("Cancel");
        playerTypeAction = InputSystem.actions.FindAction("Type");

    }

    private void Update()
    {
        playerMoveAmount = playerMoveAction.ReadValue<Vector2>();
        if (!inMonitor){
            PlayerMoveAndRotate();
            Jump();
            Look();
        }
        Interact();

    }
    private void LateUpdate()
    {
        //camera smoothing goes here
        if (SmoothCamera && !inMonitor)
        {
            SmoothLook();
        }
    }
    private void FixedUpdate()
    {
        Ray ray = playerRaycaster.eventCamera.ScreenPointToRay(lookAngle);
        looking = Physics.Raycast(ray, out playerLooked, 100f, 1<<3);
        if (looking && !inMonitor)
        {
            // Debug.Log("Hit Object: " + playerLooked.transform.gameObject.name);
            if (playerLooked.collider.CompareTag("TerminalObject") && !playerUI.enabled)
            {
                Debug.Log("Hit terminal");
                playerUI.enabled = true;
                termlooked = playerLooked.collider.GetComponent<Terminal>();
            }
        }
        else if (playerUI.enabled)
        {
            playerUI.enabled = false;
            looking = false;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus && !inMonitor)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    private void PlayerMoveAndRotate()
    {
        Vector3 moveDirection = (playerCamera.transform.right * playerMoveAmount.x) + (playerCamera.transform.forward * playerMoveAmount.y);
        if (!playerSprintAction.IsPressed()){
            playerCharacterController.Move(new Vector3(moveDirection.x, verticalVelocity, moveDirection.z) * playerWalkSpeed * Time.deltaTime);
        }
        else
        {
            playerCharacterController.Move(new Vector3(moveDirection.x, verticalVelocity, moveDirection.z) * playerWalkSpeed * Time.deltaTime * sprintSpeed);
        }
    }

    private void Jump()
    {
        if(playerCharacterController.isGrounded)
        {
            verticalVelocity = -1f;
            if (playerJumpAction.WasPressedThisFrame())
            {
                verticalVelocity = jumpHeight;
            }
        }
        else 
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void Look()
    {
        // Debug.Log(playerLookAction);
        lookAngle = playerLookAction.ReadValue<Vector2>();
        if (lookAngle != Vector2.zero) {
            // Debug.Log(lookAngle);
            Vector3 newPlayerAngle = playerCharacterController.transform.localEulerAngles;
            // Debug.Log(newPlayerAngle);
            newPlayerAngle.x += lookAngle.x;
            newPlayerAngle.y -= lookAngle.y;
            Vector3 newCameraRotation = playerCamera.localEulerAngles;
            // Debug.Log(playerCamera.eulerAngles);
            newCameraRotation.y += lookAngle.x;
            newCameraRotation.x -= lookAngle.y;
            //newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, 0, 26);
            if (!SmoothCamera){
                playerCamera.transform.localRotation = Quaternion.AngleAxis(newCameraRotation.y, Vector3.up)*Quaternion.AngleAxis(newCameraRotation.x, Vector3.right);

                playerCharacterController.transform.localRotation = Quaternion.AngleAxis(newCameraRotation.y, Vector3.up);
            }
            else
            {
                lookTarget = Quaternion.AngleAxis(newCameraRotation.y, Vector3.up)*Quaternion.AngleAxis(newCameraRotation.x, Vector3.right);
            }

        }
    }
    private void SmoothLook()
    {
        playerCamera.rotation = Quaternion.Lerp(playerCamera.rotation, lookTarget, playerRotateDampening*Time.deltaTime);
    }
    private void Interact()
    {
        if (playerInteractAction.WasPressedThisFrame())
        {
            Debug.Log("player pressed interact");
            if (looking && playerUI.enabled)
            {
                inMonitor = true;
                termlooked.Focus(playerCamera);
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                unsub = InputSystem.onAnyButtonPress.Call(termlooked.processInput);
            }
        }
        if (inMonitor && playerCancelAction.WasPressedThisFrame())
        {
            termlooked.Unfocus(playerCamera);
            inMonitor=false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            unsub?.Dispose();
            return;
        }
        // if (inMonitor && playerTypeAction.WasPressedThisFrame())
        // {
        // }
        // if ()
        // {
        //     Debug.Log("triggered?");
        // }
    }
    // void OnTriggerEnter(Collider other)
    // {
    // }
}   