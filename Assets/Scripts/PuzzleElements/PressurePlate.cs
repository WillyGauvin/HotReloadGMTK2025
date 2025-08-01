using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] Bridge myBridge;

    private void OnTriggerEnter(Collider other)
    {
        myBridge.SteppedOn(other);
    }

    private void OnTriggerExit(Collider other)
    {
        myBridge.SteppedOff(other);
    }
}
