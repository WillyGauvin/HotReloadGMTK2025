using UnityEngine;

[CreateAssetMenu(fileName = "Character Controller Parameters", menuName = "Scriptable Object/Character Controller Parameters")]
public class CharacterControllerParametersAsset : ScriptableObject
{
    [Header("Ground Movement")]
    [Min(0f)] public float MoveMaxSpeed = 3.5f;
    [Min(0f)] public float MoveAccel = 8.0f;
    [Min(0f)] public float MoveBrake = 8.0f;
    public bool InstantAcceleration = false;
    [Range(0f, 1f)] public float MaxSlope = 0.2f;

    [Header("Air Movement")]
    [Min(0f)] public float Gravity = 16.0f;
    [Min(0f)] public float MaxFallSpeed = 32.0f;
    [Min(0f)] public float MidairAccelFactor = 1.0f;
    [Min(0f)] public float MidairBrakeFactor = 1.0f;
    public float PeakVelocityMin = 0.0f;
    public float PeakVelocityMax = 0.0f;
    [Min(0f)] public float PeakGravityFactor = 1.0f;
    [Min(0f)] public float FallGravityFactor = 1.0f;

    [Header("Jump")]
    [Min(0f)] public float JumpStrength = 8.0f;
    [Min(0f)] public float LateJumpTimeSec = 0.15f;
    [Min(0f)] public float EarlyJumpTimeSec = 0.15f;
    public bool JumpInterruptionAllowed = false;
    [Min(0f)] public float JumpInterruptionVelocity = 0.0f;
    [Min(0)] public int ExtraJumpCount = 0;
    [Min(0f)] public float ExtraJumpStrengthFactor  = 1.0f;
    public bool ExtraJumpAllowInterruption = true;
}
