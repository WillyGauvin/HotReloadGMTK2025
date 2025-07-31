using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockGrabber : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform positionCastCenter;
    [SerializeField] private Transform positionWhenHeld;

    [SerializeField] private InputActionReference movementInputAction;
    [SerializeField] private InputActionReference grabInputAction;

    [Header("Parameters")]
    [SerializeField] private float grabRadius;
    [SerializeField] private bool useMouse;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float throwMinHorizontal;
    [SerializeField] private float throwMaxHorizontal;
    [SerializeField] private float throwMinVertical;
    [SerializeField] private float throwMaxVertical;
    [SerializeField, Range(0f, 1f)] private float throwInterruptionVelocity;
    [SerializeField, Range(0f, 10f)] private float throwInterruptionInvalidationTime;


    private InstructionBlock grabbedBlock;
    private InstructionBlock lastThrownBlock;
    private float lastThrownTimeSec;

    private void Update()
    {
        if (grabbedBlock)
        {
            grabbedBlock.transform.position = positionWhenHeld.position;
            if (grabInputAction.action.WasPressedThisFrame())
            {
                Ungrab();
            }
        }
        else if (grabInputAction.action.WasPressedThisFrame())
        {
            Grab();
        }
        else if (grabInputAction.action.WasReleasedThisFrame() && (Time.time - lastThrownTimeSec) < throwInterruptionInvalidationTime)
        {
            var blockBody = lastThrownBlock.GetComponent<Rigidbody>();
            blockBody.linearVelocity = new Vector3(
                blockBody.linearVelocity.x * throwInterruptionVelocity,
                blockBody.linearVelocity.y * (blockBody.linearVelocity.y < 0f ? 1f : throwInterruptionVelocity),
                blockBody.linearVelocity.z * throwInterruptionVelocity
            );
            lastThrownBlock = null;
        }
    }

    public void Grab()
    {
        var hits = Physics.OverlapSphere(positionCastCenter.position, grabRadius, layerMask, QueryTriggerInteraction.Ignore);
        InstructionBlock nearest = null;
        var nearestDist = float.PositiveInfinity;
        foreach (var item in hits)
        {
            if ((item.transform.position - positionCastCenter.position).sqrMagnitude < nearestDist && item.TryGetComponent<InstructionBlock>(out var result))
            {
                nearestDist = (item.transform.position - positionCastCenter.position).sqrMagnitude;
                nearest = result;
            }
        }
        if (nearest)
        {
            Grab(nearest);
        }
    }

    public void Grab(InstructionBlock block)
    {
        block.Body.linearVelocity = Vector2.zero;
        block.Body.isKinematic = true;

        block.GetComponent<Collider>().enabled = false;
        lastThrownBlock = null;
        grabbedBlock = block;
    }

    public void Ungrab()
    {
        var resultV = new Vector3();
        var heldInput = movementInputAction.action.ReadValue<Vector2>();
        if (useMouse)
        {
            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, transform.position.z - cam.transform.position.z));
            resultV = Vector3.Scale((transform.position - worldPos).normalized, new Vector3(
                Mathf.Lerp(throwMinHorizontal, throwMaxHorizontal, 1f + heldInput.y),
                Mathf.Lerp(throwMinVertical, throwMaxVertical, 1f + heldInput.y),
                0f
            ));
        }
        else
        {
            if (heldInput.y < 0f)
            {
                resultV = new Vector3(
                    throwMinHorizontal * heldInput.x * (1f + heldInput.y),
                    throwMinVertical * (1f + heldInput.y),
                    0f
                );
            }
            else
            {
                resultV = new Vector3(
                    heldInput.x == 0f ? 0f : Mathf.Lerp(throwMinHorizontal, throwMaxHorizontal, Mathf.Abs(heldInput.x)) * Mathf.Sign(heldInput.x),
                    Mathf.Lerp(throwMinVertical, throwMaxVertical, heldInput.y),
                    0f
                );
            }
        }
        grabbedBlock.Body.isKinematic = false;
        grabbedBlock.Body.linearVelocity = resultV;

        grabbedBlock.GetComponent<Collider>().enabled = true;
        lastThrownTimeSec = Time.time;
        lastThrownBlock = grabbedBlock;
        grabbedBlock = null;
    }
}
