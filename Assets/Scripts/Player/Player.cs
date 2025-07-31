using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsControllable = true;

    float moveSpeed = 5.0f;


    float interactRange = 2f;
    IInteractable interactable;
    public Transform CarryPosition;
    GameObject CarryObject;

    PlayerController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsControllable) return;

        if (controller.MoveInput.magnitude > 0)
        {
            transform.LookAt(transform.position + new Vector3(controller.MoveInput.x, 0, controller.MoveInput.y));
        }

        InteractTrace();

        Move(controller.MoveInput);
    }

    private void FixedUpdate()
    {
    }

    public void Move(Vector2 input)
    {
        if (!IsControllable) return;

        Vector3 Velocity = new Vector3(input.x * moveSpeed, 0.0f, input.y * moveSpeed);
        transform.position += Velocity * Time.deltaTime;

        //rb.linearVelocity = Velocity;

        //if (rb.linearVelocity.magnitude > moveSpeed)
        //{
        //    Vector3 Direction = rb.linearVelocity.normalized;
        //    Debug.Log("Limiting");
        //    rb.linearVelocity = Direction * moveSpeed;
        //}
    }

    public void Interact()
    {
        if (!IsControllable) return;

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

    //public void Teleport(Transform targetTransform)
    //{
    //    RigidbodyInterpolation prevInterpolation = rb.interpolation;

    //    rb.interpolation = RigidbodyInterpolation.None;

    //    rb.linearVelocity = Vector3.zero;
    //    rb.angularVelocity = Vector3.zero;

    //    rb.position = targetTransform.position;
    //    rb.rotation = targetTransform.rotation;

    //    Physics.SyncTransforms();

    //    rb.interpolation = prevInterpolation; ;
    //}
}
