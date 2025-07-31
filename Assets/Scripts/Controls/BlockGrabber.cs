using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockGrabber : MonoBehaviour
{
    [SerializeField] private Transform positionCastCenter;
    [SerializeField] private Transform positionWhenHeld;

    [SerializeField] private InputActionReference movementInputAction;
    [SerializeField] private InputActionReference grabInputAction;

    [Header("Parameters")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float throwMinHorizontal;
    [SerializeField] private float throwMaxHorizontal;
    [SerializeField] private float throwMinVertical;
    [SerializeField] private float throwMaxVertical;
    [SerializeField] private Vector2 throwMultiplier;


    private InstructionBlock grabbedBlock;

    private void Start()
    {
        grabInputAction.action.performed += (context) =>
        {
            if (context.performed)
            {
                if (!grabbedBlock) Grab();
                else Ungrab();
            }
        };
    }

    private void Update()
    {
        if (grabbedBlock)
        {
            grabbedBlock.transform.position = positionWhenHeld.position;
        }
    }

    public void Grab()
    {
        var hits = Physics.OverlapSphere(positionCastCenter.position, radius, layerMask, QueryTriggerInteraction.Ignore);
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
        grabbedBlock = block;
    }

    public void Ungrab()
    {
        var resultV = new Vector3();
        var heldInput = movementInputAction.action.ReadValue<Vector2>();
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
                Mathf.Lerp(throwMinHorizontal, throwMaxHorizontal, heldInput.x) * Mathf.Sign(heldInput.x),
                Mathf.Lerp(throwMinVertical, throwMaxVertical, heldInput.y),
                0f
            );
        }
        grabbedBlock.Body.isKinematic = false;
        grabbedBlock.Body.linearVelocity = resultV;

        grabbedBlock.GetComponent<Collider>().enabled = true;
        grabbedBlock = null;
    }
}
