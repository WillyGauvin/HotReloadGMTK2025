using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBeltManager : MonoBehaviour
{
    //public static ConveyorBeltManager instance { get; private set; }

    //[SerializeField] private ConveyorBelt initalBelt;

    //[SerializeField] private InputReader inputReaderPrefab;
    //private InputReader inputReader;

    //public float conveyorBeltDelay = 1.5f;
    //public float conveyorBeltMaxDelay = 3.0f;
    //public float converyorBeltMinDelay = 0.5f;

    //private void Awake()
    //{
    //    if (instance != null)
    //    {
    //        Debug.LogError("Found more then one ConveyorBelt Manager in the scene");

    //    }

    //    instance = this;
    //}
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    inputReader = Instantiate(inputReaderPrefab, transform.position, Quaternion.identity);

    //    inputReader.Initialize(initalBelt);
    //}

    ////Speed up the reader by lowering the delay
    //public void SpeedUpReader()
    //{
    //    conveyorBeltDelay -= 0.5f;

    //    if (conveyorBeltDelay <= converyorBeltMinDelay)
    //    {
    //        conveyorBeltDelay = converyorBeltMinDelay;
    //    }
    //}

    ////Slow down the reader by raising the delay
    //public void SlowDownReader()
    //{
    //    conveyorBeltDelay += 0.5f;

    //    if (conveyorBeltDelay >= conveyorBeltMaxDelay)
    //    {
    //        conveyorBeltDelay = conveyorBeltMaxDelay;
    //    }
    //}

}
