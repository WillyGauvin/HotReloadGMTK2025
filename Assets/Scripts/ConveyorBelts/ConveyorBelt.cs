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
    [SerializeField] GameObject ghostBlock;

    public CommandBlock HeldBox 
    { 
        get { return heldBox; } 
        set 
        { 
            heldBox = value; 
            if (heldBox != null)
            {
                ghostBlock.SetActive(false);
            }
        }
    }

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
            if (!box.IsTeleporting)
            {
                PickupBox(box, false);
            }
        }
    }

    public void PickupBox(CommandBlock box, bool isSwapping)
    {
        if (HeldBox != null && !isSwapping)
            return;

        HeldBox = box.Pickup(this);
    }

    public void Interact(Player player)
    {
        //Swap box
        if (HeldBox != null && player.CarryObject != null)
        {
            CommandBlock block = HeldBox;
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
        else if (HeldBox != null)
        {
            player.Pickup(HeldBox);
        }
    }

    public void LookAt(Player player)
    {
        if (HeldBox == null && player.CarryObject != null)
        {
            ghostBlock.SetActive(true);
        }
    }

    public void LookAway(Player player)
    {
        ghostBlock.SetActive(false);
    }
}
