using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using FMOD.Studio;

public class Player : MonoBehaviour
{
    public bool IsControllable = true;
    public bool isInBrain = true;

    float moveSpeed = 5.0f;
    public float rotateEasingSpeed = 5.0f;
    public float leanBackAmount = 15.0f;
    public float bounceSpeed = 15.0f;
    public float bounceStrength = 0.5f;
    public float bounceEasingSpeed = 5.0f;

    float interactRadius = 0.5f;
    IInteractable interactable;
    public Transform PlayerCarryPosition;
    public CommandBlock CarryObject;

    PlayerController controller;

    public ParticleSystem moveParticle;
    public Transform characterMesh;

    Vector3 currentForward = Vector3.zero;

    [SerializeField] LayerMask interactableMask;

    float oldSin = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsControllable)
        {
            currentForward = Vector3.zero;
            return;
        }

        Vector3 forwardThisFrame = new Vector3(controller.MoveInput.x, 0, controller.MoveInput.y);
        Vector3 rightThisFrame = Vector3.Cross(forwardThisFrame, Vector3.up);

        bool movingThisFrame = !Vector3.Equals(forwardThisFrame, Vector3.zero);

        // If no input, then don't update the forward so we keep facing that direction
        if (movingThisFrame)
        {
            currentForward = forwardThisFrame;
        }
        // Smoothly rotate towards target each frame if non-zero
        if (!Vector3.Equals(currentForward, Vector3.zero))
        {
            Vector3 targetDirection = currentForward;
            // Add some lean
            if (movingThisFrame)
            {
                targetDirection = Quaternion.AngleAxis(leanBackAmount, rightThisFrame) * targetDirection;
            }
            Quaternion target = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotateEasingSpeed * Time.deltaTime);
        }

        // Add some bounce to the character
        Vector3 meshTarget = Vector3.zero;
        if (movingThisFrame)
        {
            float newSin = (Mathf.Sin(Time.time * bounceSpeed));
            meshTarget.y += newSin * bounceStrength;

            if (newSin < 0.0f && oldSin > 0.0f)
            {
                if (isInBrain)
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.player_footsteps_tank);
                else
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.player_footsteps_outside);


                if (Random.Range(0, 101) < 25)
                {
                    if (CarryObject)
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.player_walkBox);
                    }
                    else
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.player_walk);
                    }
                }
            }
            oldSin = newSin;

        }
        else
        {
            meshTarget = Vector3.zero;
        }
        characterMesh.transform.localPosition = Vector3.Lerp(characterMesh.transform.localPosition, meshTarget, Time.deltaTime * bounceEasingSpeed);

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
        Vector3 origin = transform.position + transform.forward * interactRadius;
        Collider[] hits = Physics.OverlapSphere(origin, interactRadius, interactableMask, QueryTriggerInteraction.Collide);

        if (hits.Length == 0)
        {
            interactable?.LookAway(this);
            interactable = null;
            return;
        }

        IInteractable bestCandidate = null;
        float bestScore = float.MinValue;

        foreach (Collider hit in hits)
        {
            if (!hit.TryGetComponent(out IInteractable candidate))
                continue;


            Vector3 toTarget = (hit.transform.position - transform.position);
            toTarget.y = 0.0f;

            float distance = toTarget.magnitude;

            Vector3 direction = toTarget.normalized;
;
            float alignment = Vector3.Dot(transform.forward, direction); // 1 = perfectly in front

            if (alignment < Mathf.Cos(40f * Mathf.Deg2Rad))
                continue;

            float score = (alignment * 1.0f) + (1.0f / distance * 0.5f); // tune weights

            if (score > bestScore)
            {
                bestScore = score;
                bestCandidate = candidate;
            }
        }

        if (bestCandidate != null)
        {
            if (interactable != bestCandidate)
            {
                interactable?.LookAway(this);
                interactable = bestCandidate;
                interactable.LookAt(this);
            }
        }
        else
        {
            interactable?.LookAway(this);
            interactable = null;
        }
    }

    public void Pickup(CommandBlock interactable)
    {
        if (CarryObject != null)
            Drop();
        
        if (!interactable.IsTweening)
        {
            CarryObject = interactable.Pickup(this);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.player_pick_up, transform.position);
        }
    }

    public void Throw()
    {
        if (CarryObject == null) return;
        CarryObject.transform.SetParent(null);
        CarryObject.GetComponent<Rigidbody>().isKinematic = false;
        CarryObject.GetComponent<BoxCollider>().enabled = true;
        CarryObject.GetComponent<Rigidbody>().AddForce(transform.forward * 15.0f, ForceMode.Impulse);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.player_throw);


        CarryObject = null;

        //AudioManager.instance.PlayOneShot(FMODEvents.instance.item, transform.position);
    }

    void Drop()
    {
        if (CarryObject == null) return;
        CarryObject.transform.SetParent(null);
        CarryObject.GetComponent<Rigidbody>().isKinematic = false;
        CarryObject.GetComponent<BoxCollider>().enabled = true;
        CarryObject.GetComponent<Rigidbody>().AddForce(transform.forward * 3.0f, ForceMode.Impulse);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.player_drop);

        CarryObject = null;
    }
}
