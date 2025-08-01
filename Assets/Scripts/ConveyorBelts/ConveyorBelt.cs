using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IInteractable
{
    public Transform holdTransform;
    public CommandBlock heldBox;

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
            PickupBox(box, false);
        }
    }

    private void PickupBox(CommandBlock box, bool isSwapping)
    {
        if (heldBox != null && !isSwapping)
            return;

        heldBox = box;
        heldBox.GetComponent<BoxCollider>().enabled = true;
        Rigidbody boxRb = heldBox.GetComponent<Rigidbody>();
        boxRb.isKinematic = true;
        boxRb.angularVelocity = Vector3.zero;
        boxRb.linearVelocity = Vector3.zero;

        heldBox.transform.SetParent(holdTransform);
        heldBox.transform.localPosition = Vector3.zero;
        heldBox.transform.rotation = transform.rotation;

        box.transform.SetParent(holdTransform);
    }

    public void Interact(Player player)
    {
        Debug.Log("Interacted with Belt");
        //Swap box
        if (heldBox != null && player.CarryObject != null)
        {
            CommandBlock box = heldBox;
            PickupBox(player.CarryObject, true);
            player.CarryObject = null;
            player.Pickup(box);
        }

        //Belt Pickup box
        else if (player.CarryObject != null)
        {
            Debug.Log("Carry Object is: " + player.CarryObject);
            PickupBox(player.CarryObject, false);
            player.CarryObject = null;
        }

        //Player Pickup box
        else if (heldBox != null)
        {
            player.Pickup(heldBox);
            heldBox = null;
        }
    }

    public void LookAt()
    {
        //GetComponent<Renderer>().material.color = Color.red;
    }

    public void LookAway()
    {
        //GetComponent<Renderer>().material.color = Color.blue;
    }
}
