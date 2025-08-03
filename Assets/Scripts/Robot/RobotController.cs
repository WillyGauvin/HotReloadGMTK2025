using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

[System.Serializable]
public enum RobotExpression
{
    Neutral,
    Angry,
    Happy,
    Ouch,
}

public class RobotController : MonoBehaviour
{
    public static RobotController instance { get; private set; }

    private Queue<InputType> regularQueue = new Queue<InputType>();
    private Queue<InputType> priorityQueue = new Queue<InputType>();

    Rigidbody rb;

    [SerializeField] LayerMask collidableSurfaces;

    public List<ParticleSystem> moveParticles = new List<ParticleSystem>();
    public ParticleSystem hitParticle;

    public MeshRenderer robotBulb;
    public Color bulbMoveColor;
    private Color bulbStationaryColor;
    private bool hasReachedGoal;

    public Material expression_neutral;
    public Material expression_angry;
    public Material expression_happy;
    public Material expression_ouch;
    public MeshRenderer faceMesh;
    Coroutine expressionCoroutine = null;

    [Header("Debug Prefabs")]
    [SerializeField] GameObject ForwardBlockPrefab;
    [SerializeField] GameObject ClockwisePrefab;
    [SerializeField] GameObject CounterClockwisePrefab;
    [SerializeField] GameObject PopPrefab;


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
        if (!hasReachedGoal)
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

            if (priorityQueue.Count > 0)
            {
                input = priorityQueue.Dequeue();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.command_instantly_executed);
            }
            else
            {
                input = regularQueue.Dequeue();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.command_executed);
            }

            switch (input)
            {
                case InputType.Forward:
                    if (CanMove())
                    {
                        ActivateMoveParticles(true);
                        ActivateBulb(true);
                        Tween moveXTween = rb.DOMoveX((rb.transform.position + transform.forward * 3.0f).x, 1.0f);
                        Tween moveZTween = rb.DOMoveZ((rb.transform.position + transform.forward * 3.0f).z, 1.0f);
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.tank_driving);
                        yield return moveXTween.WaitForCompletion();
                        yield return moveZTween.WaitForCompletion();
                        ActivateMoveParticles(false);
                        ActivateBulb(false);
                    }
                    else
                    {
                        ActivateBulb(true);
                        ActivateHitParticle();
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.robot_hitwall);
                        Tween shake = transform.DOShakePosition(0.5f, transform.forward * 0.25f, 10, 40.0f, false, true, ShakeRandomnessMode.Harmonic);
                        yield return shake.WaitForCompletion();
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.voice_wallHit);
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
                case InputType.Pop:
                    InputReader.instance.PopBlocks();
                    break;
            }
        }
    }

    public void HasReachedGoal(bool state)
    {
        hasReachedGoal = state;
    }
    private bool CanMove()
    {
        Ray ray = new Ray(transform.position + new Vector3(0.0f, 0.5f, 0.0f), transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3.0f, collidableSurfaces))
        {
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

    public void SetExpression(RobotExpression expression)
    {
        if (expressionCoroutine != null)
        {
            SetExpressionMat(expression_neutral);
            StopCoroutine(expressionCoroutine);
        }
        expressionCoroutine = StartCoroutine(SetExpression_Async(expression));
    }

    private IEnumerator SetExpression_Async(RobotExpression expression)
    {
        switch (expression)
        {
            case RobotExpression.Neutral:
                SetExpressionMat(expression_neutral);
                break;
            case RobotExpression.Angry:
                SetExpressionMat(expression_angry);
                break;
            case RobotExpression.Happy:
                SetExpressionMat(expression_happy);
                break;
            case RobotExpression.Ouch:
                SetExpressionMat(expression_ouch);
                break;
        }
        yield return new WaitForSeconds(1.0f);
        SetExpressionMat(expression_neutral);
    }

    private void SetExpressionMat(Material mat)
    {
        Material[] materials = faceMesh.materials;
        materials[1] = mat;
        faceMesh.materials = materials;
        RobotBrain.instance.SetExpression(mat);
    }

    void ActivateHitParticle()
    {
        if (hitParticle.isPlaying)
        {
            hitParticle.Stop();
        }
        hitParticle.Play();

        SetExpression(RobotExpression.Ouch);
    }

    public void DEBUG_SpawnBlock(InputType type)
    {
        GameObject prefab;

        switch (type)
        {
            case InputType.Forward:
                prefab = ForwardBlockPrefab;
                break;
            case InputType.Rotate_Clockwise:
                prefab = ClockwisePrefab;
                break;
            case InputType.Rotate_CounterClockwise:
                prefab = CounterClockwisePrefab;
                break;
            case InputType.Pop:
                prefab = PopPrefab;
                break;
            default:
                prefab = ForwardBlockPrefab;
                break;
        }

        Object.Instantiate(prefab, transform.position + new Vector3(0.0f, 4.0f, 0.0f), transform.rotation);
    }
}

