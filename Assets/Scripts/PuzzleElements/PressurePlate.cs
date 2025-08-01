using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] Bridge myBridge;

    private HashSet<GameObject> validObjectsInTrigger = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidObject(other))
        {
            validObjectsInTrigger.Add(other.gameObject);

            if (validObjectsInTrigger.Count == 1)
            {
                myBridge.OpenBridge();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsValidObject(other))
        {
            validObjectsInTrigger.Remove(other.gameObject);

            if (validObjectsInTrigger.Count == 0)
            {
                myBridge.CloseBridge();
            }
        }
    }

    private bool IsValidObject(Collider collider)
    {
        return collider.GetComponent<Player>() || collider.GetComponent<RobotController>();
    }
}
