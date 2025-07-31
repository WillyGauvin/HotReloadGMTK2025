using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockGrabber : MonoBehaviour
{
    [SerializeField] private Rigidbody controllerBody;
    [SerializeField] private Transform positionCastCenter;
    [SerializeField] private Transform positionWhenHeld;

    [SerializeField] private InputActionReference inputAction;

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
        inputAction.action.performed += (context) =>
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
        var resultV = Vector3.Scale(controllerBody.linearVelocity, throwMultiplier);
        if (resultV.x != 0f)
        {
            resultV.x = Mathf.Clamp(MathF.Abs(resultV.x), throwMinHorizontal, throwMaxHorizontal) * Mathf.Sign(resultV.x);
        }
        resultV.y = Mathf.Clamp(resultV.y, throwMinVertical, throwMaxVertical);
        grabbedBlock.Body.isKinematic = false;
        grabbedBlock.Body.linearVelocity = resultV;

        grabbedBlock.GetComponent<Collider>().enabled = true;
        grabbedBlock = null;
    }
}
