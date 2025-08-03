using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ConveyorBelt : MonoBehaviour, IInteractable
{
    public Transform beltholdTransform;
    public CommandBlock heldBox;
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject ghostBlock;
    private bool isPopping = false;

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
                Debug.Log(gameObject.name + " Could not find next belt");
                return null;
        }
        Debug.Log(gameObject.name + " Could not find next belt");
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

    private void Update()
    {

        if (HeldBox)
        {
            RaycastHit[] hits = Physics.SphereCastAll(beltholdTransform.position, 0.25f, beltholdTransform.up, 0.25f, LayerMask.GetMask("Box"));
            bool doesHaveBlock = false;
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.transform.parent == beltholdTransform)
                {
                    doesHaveBlock = true;
                    break;
                }
            }
            if (!doesHaveBlock)
            {
                HeldBox = null;
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

    public void PopBlock()
    {
        StartCoroutine(PopBlockAnimation());
    }

    IEnumerator PopBlockAnimation()
    {
        if (heldBox)
        {
            if (!isPopping)
            {
                isPopping = true;
                heldBox.transform.SetParent(null);

                Tween popTween = heldBox.transform.DOJump(beltholdTransform.position + beltholdTransform.right * 4.0f, 3.0f, 1, 1.0f);
                GetNextBelt().PopBlock();
                yield return popTween.WaitForCompletion();

                if (heldBox)
                {
                    heldBox.GetComponent<Rigidbody>().isKinematic = false;
                    heldBox.GetComponent<BoxCollider>().enabled = true;
                    heldBox = null;
                }
                isPopping = false;
            }

        }
        yield return null;

    }
}
