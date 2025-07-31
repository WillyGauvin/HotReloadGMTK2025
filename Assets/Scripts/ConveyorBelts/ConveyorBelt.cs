using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Transform holdTransform;
    [HideInInspector] public CommandBlock heldBox;

    public ConveyorBelt GetNextBelt()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.0f))
        {
            if (hit.collider.TryGetComponent<ConveyorBelt>(out ConveyorBelt nextBelt))
                return nextBelt;
            else
                return null;
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CommandBlock>(out CommandBlock box))
        {
            PickupBox(box);
        }
    }

    private void PickupBox(CommandBlock box)
    {
        if (heldBox != null)
            return;

        heldBox = box;
        Rigidbody boxRb = heldBox.GetComponent<Rigidbody>();
        boxRb.isKinematic = true;
        boxRb.angularVelocity = Vector3.zero;
        boxRb.linearVelocity = Vector3.zero;

        heldBox.transform.SetParent(holdTransform);
        heldBox.transform.localPosition = Vector3.zero;
        heldBox.transform.rotation = transform.rotation;

        box.transform.SetParent(holdTransform);
    }
}
