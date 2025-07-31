using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBeltManager : MonoBehaviour
{
    public static ConveyorBeltManager instance { get; private set; }

    [SerializeField] private ConveyorBelt InitalBelt;

    public List<ConveyorBelt> beltList;

    [SerializeField] private InputReader inputReader;

    public float conveyorBeltDelay = 1.5f;
    public float conveyorBeltMaxDelay = 3.0f;
    public float converyorBeltMinDelay = 0.5f;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more then one ConveyorBelt Manager in the scene");

        }

        instance = this;

        beltList = new List<ConveyorBelt>();

        if (InitalBelt != null)
        {
            beltList.Add(InitalBelt);

            bool isAlreadyAdded = false;
            do
            {
                //find the next belt using lastest belt added in the list
                ConveyorBelt belt = beltList.Last().FindNextBelt();

                //if this belt and the first belt are the same, end the loop
                if (belt == InitalBelt)
                {
                    isAlreadyAdded = true;
                }
                //otherwise add it to the list and loop
                else
                {
                    beltList.Add(belt);
                }

            } while (!isAlreadyAdded);
        }
        else
        {
            Debug.LogError("Have Not Set Inialt Belt in Conveyor Belt Manager");

        }

        //spawn a input reader and set it up
        if (inputReader != null)
        {
            InputReader newReader = Instantiate(inputReader, transform.localPosition, Quaternion.identity);
            inputReader = newReader;
            inputReader.SetUpInputReader(InitalBelt);
        }
        else
            Debug.LogError("Have Not Set inputReader prefab reference in Conveyor Belt Manager");

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //start the loop if everything is working and has been properly setup
        if (inputReader != null && beltList.Count > 0)
        {
            StartCoroutine(MoveInputReader());

        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator MoveInputReader()
    {

        while (true)
        {
            //find current index of the input reader
            int currentBeltIndex = beltList.IndexOf(inputReader.currentBelt);

            //Check if belt is valid
            if (currentBeltIndex != -1)
            {
                //check if its the last belt in the list
                if (currentBeltIndex == beltList.Count - 1)
                {
                    inputReader.SetNewBelt(InitalBelt);
                }
                //otherwise, move to the next one
                else
                {
                    inputReader.SetNewBelt(beltList[currentBeltIndex + 1]);
                }

            }

            yield return new WaitForSeconds(conveyorBeltDelay);
        }
    }

    //Speed up the reader by lowering the delay
    public void SpeedUpReader()
    {
        conveyorBeltDelay -= 0.5f;

        if (conveyorBeltDelay <= converyorBeltMinDelay)
        {
            conveyorBeltDelay = converyorBeltMinDelay;
        }
    }

    //Slow down the reader by raising the delay
    public void SlowDownReader()
    {
        conveyorBeltDelay += 0.5f;

        if (conveyorBeltDelay >= conveyorBeltMaxDelay)
        {
            conveyorBeltDelay = conveyorBeltMaxDelay;
        }
    }

}
