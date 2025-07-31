using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private static int ConveyorBeltID = 0;

    public ConveyorBelt beltInSequence;
    public RobotInputs beltItem;
    public InputReader robotInputReader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        beltInSequence = null;
        beltInSequence = FindNextBelt();

        gameObject.name = $"Belt: {ConveyorBeltID++}";

        if (robotInputReader != null)
        {
            robotInputReader.inputReader.transform.position = GetItemPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (robotInputReader != null && robotInputReader.inputReader != null)
        {
            StartCoroutine(StartBeltMove());
        }

    }

    public Vector3 GetItemPosition()
    {
        float padding = 4f;
        Vector3 position = transform.position;

        return new Vector3(position.x, position.y + padding, position.z);
    }

    private IEnumerator StartBeltMove()
    {
        yield return new WaitForSeconds(ConveyorBeltManager.instance.ConveyorBeltDelay);

        if (robotInputReader != null && beltInSequence != null && robotInputReader.inputReader != null)
        {
            Vector3 newPos = beltInSequence.GetItemPosition();

            robotInputReader.inputReader.transform.position = newPos;

            beltInSequence.robotInputReader = robotInputReader;

            if (beltInSequence.beltItem != null && beltInSequence.robotInputReader != null)
            {
                //Call code on robot, either through reader or on manager to make the robot do something
                Debug.Log("Hit Something");
            }

            robotInputReader = null;


        }
    }

    private ConveyorBelt FindNextBelt()
    {
        Transform currentBeltTransfomr = transform; ;
        RaycastHit hit;

        Vector3 forwardDir = transform.forward;

        Ray raySent = new Ray(currentBeltTransfomr.position, forwardDir);

        if (Physics.Raycast(raySent, out hit, 1f))
        {
            ConveyorBelt nextBelt = hit.collider.GetComponent<ConveyorBelt>();
            if (nextBelt != null)
            {
                return nextBelt;
            }

        }
        return null;
    }
}
