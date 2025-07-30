using UnityEngine;

public class ConveyorBeltManager : MonoBehaviour
{
    public static ConveyorBeltManager instance { get; private set; }


    public float ConveyorBeltSpeed = 2.0f;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more then one conveyorBelt Manager in the scene");

        }

        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
