using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Player Controller in the scene.");
        }
        instance = this;
    }

    public enum ActionMap
    {
        Player,
        UI,
    }

    PlayerInput input;
    Vector2 MoveInput;
    Vector2 LookInput;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddTorque(new Vector3(MoveInput.y * Time.deltaTime * 10.0f, MoveInput.x * Time.deltaTime * 10.0f, 0.0f));
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
        RuntimeManager.PlayOneShot(FMODEvents.instance.doorOpening, transform.position);
        //Debug.Log("PlayingSound");
    }

    void OnLook(InputValue value)
    {
        LookInput = value.Get<Vector2>().normalized;
    }
}
