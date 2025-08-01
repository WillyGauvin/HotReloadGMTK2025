using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IInteractable
{
    public Transform beltholdTransform;
    public CommandBlock heldBox;
    [SerializeField] LayerMask layerMask;

    public ConveyorBelt GetNextBelt()
    {
        Ray ray = new Ray(beltholdTransform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 5.5f, layerMask))
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
        if (other.TryGetComponent<CommandBlock>(out CommandBlock box) && other.GetComponentInParent<Player>() == null)
        {
            PickupBox(box, false);
        }
    }

    private void PickupBox(CommandBlock box, bool isSwapping)
    {
        if (heldBox != null && !isSwapping)
            return;

        heldBox = box.Pickup(this);
    }

    public void Interact(Player player)
    {
        //Swap box
        if (heldBox != null && player.CarryObject != null)
        {
            CommandBlock block = heldBox;
            PickupBox(player.CarryObject, true);
            player.Pickup(block);
        }

        //Belt Pickup box
        else if (player.CarryObject != null)
        {
            PickupBox(player.CarryObject, false);
            player.CarryObject = null;
        }

        //Player Pickup box
        else if (heldBox != null)
        {
            player.Pickup(heldBox);
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
