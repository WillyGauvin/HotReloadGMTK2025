using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }

    private InputAction speedUpAction;
    PlayerInput input;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Player Controller in the scene.");
        }
        instance = this;

        input = GetComponent<PlayerInput>();

        speedUpAction = input.actions["SpeedUpRobot"];

        speedUpAction.performed += OnSpeedUp;
        speedUpAction.canceled += OnSlowDown;

    }
    private void OnDestroy()
    {
        if (speedUpAction != null)
        {
            speedUpAction.performed -= OnSpeedUp;
            speedUpAction.canceled -= OnSlowDown;
        }
    }

    public enum ActionMap
    {
        Player,
        UI,
    }

    Player player;
    public Vector2 MoveInput;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        //GetComponent<Rigidbody>().AddTorque(new Vector3(MoveInput.y * Time.deltaTime * 10.0f, MoveInput.x * Time.deltaTime * 10.0f, 0.0f));
    }
    void Update()
    {
    }

    public void SwitchToActionMap(ActionMap map)
    {
        switch (map)
        {
            case ActionMap.Player:
                input.SwitchCurrentActionMap("Player");
                break;
            case ActionMap.UI:
                input.SwitchCurrentActionMap("UI");
                break;
        }
    }


    void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            MenuManager.instance.Pause();
        }
    }

    void OnUnPause(InputValue value)
    {
        if (value.isPressed)
        {
            MenuManager.instance.UnPause();
        }
    }

    void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>().normalized;
        //RuntimeManager.PlayOneShot(FMODEvents.instance.doorOpening, transform.position);
        //Debug.Log("PlayingSound");
    }

    void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            player.Interact();
        }
    }

    void OnThrow(InputValue value)
    {
        if (value.isPressed)
        {
            player.Throw();
        }
    }
    
    void OnSpeedUp(InputAction.CallbackContext context)
    {
        InputReader.instance.SpeedUpReader();
    }

    void OnSlowDown(InputAction.CallbackContext context)
    {
        InputReader.instance.SlowDownReader();

    }
    void OnDEBUG_RobotForward(InputValue value)
    {
#if UNITY_EDITOR
        RobotController.instance.ReceiveInput(InputType.Forward, false);
#endif
    }
    void OnDEBUG_RobotClockwise(InputValue value)
    {
#if UNITY_EDITOR
        RobotController.instance.ReceiveInput(InputType.Rotate_Clockwise, false);
#endif
    }

    void OnDEBUG_RobotCounterClockwise(InputValue value)
    {
#if UNITY_EDITOR
        RobotController.instance.ReceiveInput(InputType.Rotate_CounterClockwise, false);
#endif
    }

    void OnDEBUG_RobotForwardPriority(InputValue value)
    {
#if UNITY_EDITOR
        RobotController.instance.ReceiveInput(InputType.Forward, true);
#endif
    }

}
