using UnityEngine;

public enum InputType { Up, Down, Left, Right };
public class RobotInputs : MonoBehaviour
{

    public GameObject inputBox;


    public InputType inputType;

    private void Awake()
    {
        inputBox = gameObject;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public InputType GetInputType()
    {
        return inputType;
    }

}
