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
                ConveyorBelt belt = beltList.Last().FindNextBelt();

                if (belt == InitalBelt)
                {
                    isAlreadyAdded = true;
                }
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

        if (inputReader != null)
            inputReader.SetUpInputReader(InitalBelt);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            int currentBeltIndex = beltList.IndexOf(inputReader.currentBelt);


            //Check if belt is valid
            if (currentBeltIndex != -1)
            {
                //check if its the last belt in the list
                if (currentBeltIndex == beltList.Count - 1)
                {
                    inputReader.SetNewBelt(InitalBelt);
                }
                else
                {
                    inputReader.SetNewBelt(beltList[currentBeltIndex + 1]);
                }

            }

            yield return new WaitForSeconds(conveyorBeltDelay);
        }
    }

    public void SpeedUpReader()
    {
        conveyorBeltDelay -= 0.5f;

        if (conveyorBeltDelay <= converyorBeltMinDelay)
        {
            conveyorBeltDelay = converyorBeltMinDelay;
        }
    }

    public void SlowDownReader()
    {
        conveyorBeltDelay += 0.5f;

        if (conveyorBeltDelay >= conveyorBeltMaxDelay)
        {
            conveyorBeltDelay = conveyorBeltMaxDelay;
        }
    }

}
