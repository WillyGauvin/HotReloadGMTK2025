using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class RobotController : MonoBehaviour
{
    public static RobotController instance {  get; private set; }

    private Queue<InputType> regularQueue = new Queue<InputType>();
    private Queue<InputType> priorityQueue = new Queue<InputType>();

    Rigidbody rb;

    [SerializeField] LayerMask collidableSurfaces;

    public List<ParticleSystem> moveParticles = new List<ParticleSystem>();
    
    public MeshRenderer robotBulb;
    public Color bulbMoveColor;
    private Color bulbStationaryColor;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Robot Controller in scene");
        }
        else
        {
            instance = this;
        }
        bulbStationaryColor = robotBulb.material.GetColor("_BaseColor");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(MoveRobot());
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position + new Vector3(0.0f, 0.5f, 0.0f), transform.forward * 3.0f, Color.blue);
    }

    //Call when an input has been recognized by the InputReader
    public void ReceiveInput(InputType input, bool isPriority)
    {
        if (isPriority)
        {
            priorityQueue.Enqueue(input);
        }
        else
        {
            regularQueue.Enqueue(input);
        }
    }

    IEnumerator MoveRobot()
    {
        while (true)
        {
            if (priorityQueue.Count == 0 && regularQueue.Count == 0)
            {
                yield return null;
                continue;
            }

            InputType input;

            input = (priorityQueue.Count > 0) ? priorityQueue.Dequeue() : regularQueue.Dequeue();

            switch (input)
            {
                case InputType.Forward:
                    if (CanMove())
                    {
                        ActivateMoveParticles(true);
                        ActivateBulb(true);
                        Tween moveXTween = rb.DOMoveX((rb.transform.position + transform.forward * 3.0f).x, 1.0f);
                        Tween moveZTween = rb.DOMoveZ((rb.transform.position + transform.forward * 3.0f).z, 1.0f);
                        yield return moveXTween.WaitForCompletion();
                        yield return moveZTween.WaitForCompletion();
                        ActivateMoveParticles(false);
                        ActivateBulb(false);
                    }
                    else
                    {
                        ActivateBulb(true);
                        Tween shake = transform.DOShakePosition(0.5f, transform.forward * 0.25f, 10, 40.0f, false, true, ShakeRandomnessMode.Harmonic);
                        yield return shake.WaitForCompletion();
                        ActivateBulb(false);
                    }
                    break;

                case InputType.Rotate_Clockwise:
                    ActivateBulb(true);
                    Tween rotateClockwiseTween = rb.DORotate(rb.transform.rotation.eulerAngles + new Vector3(0.0f, 90.0f, 0.0f), 1.0f);
                    yield return rotateClockwiseTween.WaitForCompletion();
                    ActivateBulb(false);
                    break;

                case InputType.Rotate_CounterClockwise:
                    ActivateBulb(true);
                    Tween rotateCounterClockwiseTween = rb.DORotate(rb.transform.rotation.eulerAngles + new Vector3(0.0f, -90.0f, 0.0f), 1.0f);
                    yield return rotateCounterClockwiseTween.WaitForCompletion();
                    ActivateBulb(false);
                    break;
            }
        }
    }

    private bool CanMove()
    {
        Ray ray = new Ray(transform.position + new Vector3(0.0f, 0.5f, 0.0f), transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3.0f, collidableSurfaces))
        {
            Debug.Log(hit.collider.gameObject);
            return false;
        }
        else
            return true;
    }

    private void ActivateBulb(bool active)
    {
        robotBulb.material.SetColor("_BaseColor", active ? bulbMoveColor : bulbStationaryColor);
    }

    void ActivateMoveParticles(bool activate)
    {
        if (activate)
        {
            foreach (var particle in moveParticles)
            {
                particle.Play();
            }
        }
        else
        {
            foreach (var particle in moveParticles)
            {
                particle.Stop();
            }
        }
    }
}
