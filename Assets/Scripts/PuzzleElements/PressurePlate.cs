using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] List<Bridge> myBridge;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var bridge in myBridge)
            bridge.SteppedOn(other);
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var bridge in myBridge)
            bridge.SteppedOff(other);
    }
}
