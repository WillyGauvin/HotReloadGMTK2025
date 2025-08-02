using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;


public class InputReader : MonoBehaviour
{
    public static InputReader instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
            Debug.LogError("Found more than 1 InputReader in scene");
    }

    public ConveyorBelt initalBelt;
    public Transform BrainCenter;

    float readerMovementSpeed = 4.0f;
    float readerMaxMovementSpeed = 10.0f;
    float readerMinMovementSpeed = 4.0f;
    bool isSpedUp = false;
    ConveyorBelt currentBelt;
    Coroutine TraversBeltCoroutine;

    void Start()
    {
        currentBelt = initalBelt;
        transform.position = currentBelt.beltholdTransform.position;
        if (initalBelt == null)
            Debug.LogError("Initial Belt not set on InputReader");
        else
            TraversBeltCoroutine = StartCoroutine(TraverseTrack());
    }

    public void SpeedUpReader()
    {
        StopCoroutine(TraversBeltCoroutine);
        transform.DOKill();
        readerMovementSpeed = readerMaxMovementSpeed;
        isSpedUp = true;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.commandline_speedUp);
        TraversBeltCoroutine = StartCoroutine(TraverseTrack());
    }

    public void SlowDownReader()
    {
        if (readerMovementSpeed == readerMaxMovementSpeed)
        {
            StopCoroutine(TraversBeltCoroutine);
            transform.DOKill();
            readerMovementSpeed = readerMinMovementSpeed;
            isSpedUp = false;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.commandline_slowDown);
            TraversBeltCoroutine = StartCoroutine(TraverseTrack());
        }
    }

    private IEnumerator TraverseTrack()
    {
        while (true)
        {
            Tween moveToBelt = transform.DOMove(currentBelt.beltholdTransform.position, readerMovementSpeed).SetSpeedBased(true).SetEase(Ease.Linear);
            yield return moveToBelt.WaitForCompletion();

            if (currentBelt.HeldBox != null)
            {
                RobotController.instance.ReceiveInput(currentBelt.HeldBox.inputType, false);
            }

            currentBelt = currentBelt.GetNextBelt();

            if (!isSpedUp)
                yield return new WaitForSeconds(0.5f);
        }
    }

    public void PopBlocks()
    {
        StartCoroutine(Pop());
    }

    IEnumerator Pop()
    {
        ConveyorBelt currentBelt = initalBelt;

        currentBelt.PopBlock();

        currentBelt = currentBelt.GetNextBelt();

        while(currentBelt != initalBelt)
        {
            currentBelt.PopBlock();
            currentBelt = currentBelt.GetNextBelt();
        }

        yield return null;
    }
}
