using System;
using System.Linq;
using UnityEngine;


public class InputReader : MonoBehaviour
{

    public ConveyorBelt currentBelt;
    public RobotMovement robotMovement;

    protected float conveyorBeltHeightOffset = 2.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        robotMovement = FindAnyObjectByType<RobotMovement>();

        if (robotMovement == null)
            Debug.LogError("Have Not Set Robot in Scene");
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Called when spawned in by the Conveyor Belt Manager
    public void SetUpInputReader(ConveyorBelt startingBelt)
    {
        transform.position = startingBelt.GetItemPosition(conveyorBeltHeightOffset);
        currentBelt = startingBelt;
    }

    public void SetNewBelt(ConveyorBelt newBelt)
    {
        if (newBelt != null)
        {
            currentBelt = newBelt;
            transform.position = currentBelt.GetItemPosition(conveyorBeltHeightOffset);
            CheckForInputs();
        }


    }

    private void CheckForInputs()
    {
        if (currentBelt != null)
        {
            //Check if the belt has a heldInput
            if (currentBelt.heltInput != null)
            {
                //let the robot know its got a new input

                robotMovement.ReceiveInput(currentBelt.heltInput.GetInputType());
            }
        }
    }
}
