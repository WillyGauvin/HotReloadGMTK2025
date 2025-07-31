using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerInside : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider bodyCollider;
    [SerializeField] private CharacterControllerParametersAsset controllerParams;
    [SerializeField] private InputActionReference inputActionMovement;
    [SerializeField] private InputActionReference inputActionJump;

    [FoldoutGroup("Events")] public UnityEvent FloorJumped;
    [FoldoutGroup("Events")] public UnityEvent<int> ExtraJumped;
    [FoldoutGroup("Events")] public UnityEvent TouchedFloor;
    [FoldoutGroup("Events")] public UnityEvent LeftFloor;

    private Vector2 moveInput;
    private float moveInputLastDirection = 1f;
    private bool isOnFloor = false;
    private bool canMove = true;
    private bool jumpInterruptionAvailable = false;
    private bool earlyJumpAvailable = false;
    private bool lateJumpAvailable = false;
    private float lastGroundSec = 0.0f;
    private float lastJumpInputSec = 0.0f;
    private int extraJumpsUsed = 0;

    private readonly HashSet<Collider> _floorContacts = new();

    public float MoveInputLLastDirection => moveInputLastDirection;
    public bool CanMove
    {
        get => canMove;
        set
        {
            canMove = value;
            bodyCollider.attachedRigidbody.linearVelocity = value ? Velocity : Vector2.zero;
        }
    }
    public Vector3 Velocity {
        get => bodyCollider.attachedRigidbody.linearVelocity;
        set
        {
            var body = bodyCollider.attachedRigidbody;
            var appliedImpulse = (body.mass * value) - (body.mass * body.linearVelocity);
            body.AddForce(appliedImpulse, ForceMode.Impulse);
        }
    }

    public void Jump(bool allowMidair = false)
    {
        if (!canMove || lastJumpInputSec == Time.time)
        {
            return;
        }
        lastJumpInputSec = Time.time;
        earlyJumpAvailable = true;

        var jumpWithVelocity = controllerParams.JumpStrength;
        var currentJumpIsExtra = false;
        if (
            !isOnFloor
            && !allowMidair
            && (lastGroundSec < Time.time - controllerParams.LateJumpTimeSec || !lateJumpAvailable))
        {
            if (extraJumpsUsed >= controllerParams.ExtraJumpCount)
            {
                return;
            }
            extraJumpsUsed += 1;
            currentJumpIsExtra = true;
            jumpWithVelocity *= controllerParams.ExtraJumpStrengthFactor;
        }
        else
        {
            Velocity = new Vector3(Velocity.x, jumpWithVelocity, 0f);
        }
        jumpInterruptionAvailable = true;
        lateJumpAvailable = false;
        earlyJumpAvailable = true; // Prevents Extra Jumps simultaneous with an Early Jump.

        // Events
        if (currentJumpIsExtra)
        {
            ExtraJumped?.Invoke(controllerParams.ExtraJumpCount - extraJumpsUsed);
        }
        else
        {
            FloorJumped?.Invoke();
        }
    }

    public void JumpReleased()
    {
        if (!controllerParams.JumpInterruptionAllowed || (extraJumpsUsed != 0 && !controllerParams.ExtraJumpAllowInterruption))
        {
            return;
        }
        if (jumpInterruptionAvailable && Velocity.y > controllerParams.JumpInterruptionVelocity)
        {
            Velocity = new Vector3(Velocity.x, controllerParams.JumpInterruptionVelocity);
        }
        jumpInterruptionAvailable = false;
        // Prevents Early Jumps (only triggers if button is still held)
        // Don't prevent them if there's no jump interruption
        // (otherwise the player may quickly tap the button, having no reason to hold, not triggering the jump)
        earlyJumpAvailable = false;
    }

    public void FixedUpdate()
    {
        if (inputActionJump.action.WasPressedThisFrame())
        {
            Jump();
        }
        else if (inputActionJump.action.WasReleasedThisFrame())
        {
            JumpReleased();
        }
        moveInput = inputActionMovement.action.ReadValue<Vector2>();
        if (moveInput.x != 0f)
        {
            moveInputLastDirection = Mathf.Sign(moveInput.x);
        }
        if (!canMove)
        {
            return;
        }
        var newVelocity = Velocity;
        newVelocity = new Vector2(
            GetVelocityHorizontal(newVelocity.x),
            GetVelocityGravity(newVelocity.y)
        );
        Velocity = newVelocity;
    }

    private float GetVelocityHorizontal(float previousVelocity)
    {
        var newVelocity = previousVelocity;
        if (controllerParams.InstantAcceleration)
        {
            newVelocity = moveInput.x * controllerParams.MoveMaxSpeed;
        }
        var curAccel = controllerParams.MoveAccel;
        var curBrake = controllerParams.MoveBrake;
        if (!isOnFloor)
        {
            curAccel *= controllerParams.MidairAccelFactor;
            curBrake *= controllerParams.MidairBrakeFactor;
        }
        var speedDelta = curBrake;
        if (moveInput.x != 0.0f)
        {
            var velDirectionToTarget = Mathf.Sign(moveInput.x * controllerParams.MoveMaxSpeed - newVelocity);
            speedDelta = curAccel;
            if (velDirectionToTarget * newVelocity < 0.0f)
            {
                speedDelta += curBrake;
            }
        }
        return Mathf.MoveTowards(newVelocity, moveInput.x * controllerParams.MoveMaxSpeed, Time.deltaTime * speedDelta);
    }

    private float GetVelocityGravity(float previousVelocity)
    {
        if (isOnFloor || bodyCollider.attachedRigidbody.IsSleeping())
        {
            return previousVelocity;
        }
        var newVelocity = previousVelocity;
        var curGravity = controllerParams.Gravity;
        if (jumpInterruptionAvailable && newVelocity > controllerParams.PeakVelocityMin && newVelocity < controllerParams.PeakVelocityMax)
        {
            curGravity *= controllerParams.PeakGravityFactor;
        }
        else if (newVelocity < 0f)
        {
            if (previousVelocity >= 0f)
            {
                jumpInterruptionAvailable = false;
            }
            curGravity *= controllerParams.FallGravityFactor;
        }
        return Mathf.Max(newVelocity - Time.deltaTime * curGravity, -controllerParams.MaxFallSpeed);
    }

    public void OnCollisionEnter(Collision collision)
    {
        foreach (var x in collision.contacts)
        {
            if (x.normal.y > controllerParams.MaxSlope)
            {
                if (!_floorContacts.Contains(collision.collider))
                {
                    _floorContacts.Add(collision.collider);
                    if (!isOnFloor)
                    {
                        isOnFloor = true;
                        lateJumpAvailable = true;
                        extraJumpsUsed = 0;
                        TouchedFloor?.Invoke();
                    }
                }
            }
        }
        if (isOnFloor)
        {
            if (earlyJumpAvailable && lastJumpInputSec > Time.time - controllerParams.EarlyJumpTimeSec && Velocity.y <= 0f)
            {
                Jump();
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        var wasOnFloor = _floorContacts.Count > 0;
        _floorContacts.Remove(collision.collider);
        if (wasOnFloor != (_floorContacts.Count > 0))
        {
            isOnFloor = false;
            lastGroundSec = Time.time;
            LeftFloor?.Invoke();
        }
    }
}
