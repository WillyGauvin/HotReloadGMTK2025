using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsControllable = true;
    public bool isInBrain = true;

    float moveSpeed = 5.0f;
    public float rotateEasingSpeed = 5.0f;

    float interactRange = 2f;
    IInteractable interactable;
    public Transform CarryPosition;
    public CommandBlock CarryObject;

    PlayerController controller;

    public ParticleSystem moveParticle;

    Vector3 currentForward = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsControllable) return;

        Vector3 forwardThisFrame = new Vector3(controller.MoveInput.x, 0, controller.MoveInput.y);
        if (!Vector3.Equals(forwardThisFrame, Vector3.zero))
        {
            currentForward = forwardThisFrame;
        }

        if (!Vector3.Equals(currentForward, Vector3.zero))
        {
            Quaternion target = Quaternion.LookRotation(currentForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotateEasingSpeed * Time.deltaTime);
        }

        InteractTrace();

        Move(controller.MoveInput);
    }

    private void FixedUpdate()
    {
    }

    public void Move(Vector2 input)
    {
        if (!IsControllable)
        {
            moveParticle.Stop();
            return;
        }

        Vector3 Velocity = new Vector3(input.x * moveSpeed, 0.0f, input.y * moveSpeed);
        transform.position += Velocity * Time.deltaTime;

        if (Velocity.magnitude > 0.0f)
        {
            if (!moveParticle.isPlaying)
            {
                moveParticle.Play();
            }
        }
        else
        {
            moveParticle.Stop();
        }

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
            Drop();
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

    public void Pickup(CommandBlock interactable)
    {
        if (CarryObject != null)
            Drop();
        CarryObject = interactable;

        CarryObject.transform.SetParent(CarryPosition);
        CarryObject.transform.localPosition = Vector3.zero;
        CarryObject.transform.localRotation = Quaternion.identity;
        CarryObject.GetComponent<Rigidbody>().isKinematic = true;
        CarryObject.GetComponent<BoxCollider>().enabled = false;
    }

    public void Throw()
    {
        if (CarryObject == null) return;
        CarryObject.transform.SetParent(null);
        CarryObject.GetComponent<Rigidbody>().isKinematic = false;
        CarryObject.GetComponent<BoxCollider>().enabled = true;
        CarryObject.GetComponent<Rigidbody>().AddForce(transform.forward * 15.0f, ForceMode.Impulse);

        CarryObject = null;
    }

    void Drop()
    {
        if (CarryObject == null) return;
        CarryObject.transform.SetParent(null);
        CarryObject.GetComponent<Rigidbody>().isKinematic = false;
        CarryObject.GetComponent<BoxCollider>().enabled = true;
        CarryObject.GetComponent<Rigidbody>().AddForce(-transform.forward * 5.0f, ForceMode.Impulse);

        CarryObject = null;
    }
}
