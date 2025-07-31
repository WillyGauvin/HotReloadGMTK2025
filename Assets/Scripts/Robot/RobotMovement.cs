using UnityEngine;

public class RobotMovement : MonoBehaviour
{

    public InputReader inputReader;

    bool isHandClosed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Call when an input has been recognized by the InputReader
    public void ReceiveInput(InputType input)
    {

    }

    //When the hand is open, move the hand
    public void MoveHand(InputType input)
    {

    }

    //When the hand is closed, move the body
    public void MoveBody(InputType input)
    {

    }
}
