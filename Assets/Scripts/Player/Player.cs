using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    float moveSpeed = 5.0f;


    float interactRange = 2f;
    IInteractable interactable;
    public Transform CarryPosition;
    GameObject CarryObject;

    private Rigidbody rb;

    PlayerController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.MoveInput.magnitude > 0)
        {
            transform.LookAt(transform.position + new Vector3(controller.MoveInput.x, 0, controller.MoveInput.y));
        }

        InteractTrace();
    }

    private void FixedUpdate()
    {
        Move(controller.MoveInput);
    }

    public void Move(Vector2 input)
    {
        Vector3 Velocity = new Vector3(input.x * moveSpeed, 0.0f, input.y * moveSpeed);
        rb.linearVelocity = Velocity;

        if (rb.linearVelocity.magnitude > moveSpeed)
        {
            Vector3 Direction = rb.linearVelocity.normalized;
            Debug.Log("Limiting");
            rb.linearVelocity = Direction * moveSpeed;
        }
    }

    public void Interact()
    {
        if (interactable != null)
        {
            interactable.Interact(this);
        }
        else
        {
            Throw();
        }
    }

    void InteractTrace()
    {
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.blue);

        Ray r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (interactable != interactObj)
                {
                    interactable?.LookAway();
                    interactable = interactObj;
                    interactable.LookAt();
                }
            }
            else
            {
                interactable?.LookAway();
                interactable = null;
            }
        }
        else
        {
            interactable?.LookAway();
            interactable = null;
        }
    }

    public void Pickup(GameObject interactable)
    {
        if (CarryObject != null)
            Drop();
        CarryObject = interactable;

        CarryObject.transform.SetParent(CarryPosition);
        CarryObject.transform.localPosition = Vector3.zero;
        CarryObject.transform.localRotation = Quaternion.identity;
        CarryObject.GetComponent<Rigidbody>().isKinematic = true;
        CarryObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    void Throw()
    {
        if (CarryObject == null) return;
        CarryObject.transform.SetParent(null);
        CarryObject.GetComponent<Rigidbody>().isKinematic = false;
        CarryObject.GetComponent<BoxCollider>().isTrigger = false;
        CarryObject.GetComponent<Rigidbody>().AddForce(transform.forward * 500.0f);

        CarryObject = null;
    }
    
    void Drop()
    {
        if (CarryObject == null) return;
        CarryObject.transform.SetParent(null);
        CarryObject.GetComponent<Rigidbody>().isKinematic = false;
        CarryObject.GetComponent<BoxCollider>().isTrigger = false;
        CarryObject.GetComponent<Rigidbody>().AddForce(-transform.forward * 100.0f);

        CarryObject = null;
    }
}
