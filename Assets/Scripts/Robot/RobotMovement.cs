using Unity.VisualScripting;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{

    public InputReader inputReader;

    public GameObject robotHand;
    public GameObject robotBody;

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
        if (!isHandClosed)
        {
            MoveHand(input);
        }
        else
        {
            MoveBody(input);
        }
    }

    //When the hand is open, move the hand
    public void MoveHand(InputType input)
    {
        switch (input)
        {
            case InputType.Left:
                Debug.Log("Hit Left");

                break;
            case InputType.Right:
                Debug.Log("Hit Right");

                break;
            case InputType.Up:
                Debug.Log("Hit Up");

                break;
            case InputType.Down:
                Debug.Log("Hit Down");

                break;
        }
    }

    //When the hand is closed, move the body
    public void MoveBody(InputType input)
    {

    }
}
