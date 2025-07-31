using System;
using System.Linq;
using UnityEngine;


public class InputReader : MonoBehaviour
{
    public ConveyorBelt currentBelt;

    protected float conveyorBeltHeightOffset = 2.5f;

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

    //How the input reader moves
    public void SetNewBelt(ConveyorBelt newBelt)
    {
        if (newBelt != null)
        {
            currentBelt = newBelt;
            transform.position = currentBelt.GetItemPosition(conveyorBeltHeightOffset);
            CheckForInputs();
        }


    }

    //When called, check if the reader is on the same belt as a input
    private void CheckForInputs()
    {
        if (currentBelt != null)
        {
            //Check if the belt has a heldInput
            if (currentBelt.heltInput != null)
            {
                //let the robot know its got a new input

                RobotController.instance.ReceiveInput(currentBelt.heltInput.inputType, false);
            }
        }
    }
}
