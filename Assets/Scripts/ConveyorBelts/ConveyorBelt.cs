using System.Collections;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private static int ConveyorBeltID = 0;

    public ConveyorBelt beltInSequence;
    public RobotInputs beltItem;
    public bool isSpaceTaken;

    private ConveyorBeltManager conveyorBeltManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        beltInSequence = null;
        //beltInSequence = FindNextBelt();

        gameObject.name = $"Belt: {ConveyorBeltID++}";
    }

    // Update is called once per frame
    void Update()
    {
        if (beltInSequence == null)
        {
            //beltInSequence = FindNextBelt();
        }

        if (beltItem != null && beltItem.inputBox != null)
        {
            //StartCoroutine(StartBeltMove());
        }
    }

    public Vector3 GetItemPosition()
    {
        float padding = 0.4f;


        return Vector3.zero;
    }

    //private IEnumerator StartBeltMove()
    //{

    //}

    //private ConveyorBelt FindNextBelt()
    //{

    //}

}
