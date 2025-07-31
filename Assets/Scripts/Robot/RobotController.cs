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
                    Tween moveXTween = rb.DOMoveX((rb.transform.position + transform.forward * 3.0f).x, 1.0f);
                    Tween moveZTween = rb.DOMoveZ((rb.transform.position + transform.forward * 3.0f).z, 1.0f);
                    yield return moveXTween.WaitForCompletion();
                    yield return moveZTween.WaitForCompletion();
                    break;

                case InputType.Rotate_Clockwise:
                    Tween rotateClockwiseTween = rb.DORotate(rb.transform.rotation.eulerAngles + new Vector3(0.0f, 90.0f, 0.0f), 1.0f);
                    yield return rotateClockwiseTween.WaitForCompletion();
                    break;

                case InputType.Rotate_CounterClockwise:
                    Tween rotateCounterClockwiseTween = rb.DORotate(rb.transform.rotation.eulerAngles + new Vector3(0.0f, -90.0f, 0.0f), 1.0f);
                    yield return rotateCounterClockwiseTween.WaitForCompletion();
                    break;
            }
        }
    }
}
