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

    void Start()
    {
        if (initalBelt == null)
            Debug.LogError("Initial Belt not set on InputReader");
        else
            StartCoroutine(TraverseTrack(initalBelt));
    }

    public void SpeedUpReader()
    {
        readerMovementSpeed = readerMaxMovementSpeed;
        isSpedUp = true;
    }

    public void SlowDownReader()
    {
        readerMovementSpeed = readerMinMovementSpeed;
        isSpedUp = false;
    }

    private IEnumerator TraverseTrack(ConveyorBelt startingBelt)
    {
        ConveyorBelt currentBelt = startingBelt;
        transform.position = currentBelt.beltholdTransform.position;

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
