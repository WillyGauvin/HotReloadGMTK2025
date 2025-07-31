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

    [SerializeField] private ConveyorBelt initalBelt;

    float readerMovementSpeed = 1.5f;
    float readerMaxMovementSpeed = 3.0f;
    float readerMinMovementSpeed = 3.0f;

    void Start()
    {
        if (initalBelt == null)
            Debug.LogError("Initial Belt not set on ConveyorBeltManager");
        else
            StartCoroutine(TraverseTrack(initalBelt));
    }

    public void SpeedUpReader()
    {
        readerMovementSpeed = readerMaxMovementSpeed;
    }

    public void SlowDownReader()
    {
        readerMovementSpeed = readerMinMovementSpeed;
    }

    private IEnumerator TraverseTrack(ConveyorBelt startingBelt)
    {
        ConveyorBelt currentBelt = startingBelt;
        transform.position = currentBelt.holdTransform.position;

        while (true)
        {
            Tween moveToBelt = transform.DOMove(currentBelt.holdTransform.position, readerMovementSpeed).SetSpeedBased(true);
            yield return moveToBelt.WaitForCompletion();

            if (currentBelt.heldBox != null)
            {
                RobotController.instance.ReceiveInput(currentBelt.heldBox.inputType, false);
            }

            currentBelt = currentBelt.GetNextBelt();

            yield return new WaitForSeconds(0.5f);
        }
    }
}
