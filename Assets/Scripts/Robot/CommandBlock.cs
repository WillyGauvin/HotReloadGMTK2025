using UnityEngine;

public enum InputType { Forward, Rotate_Clockwise, Rotate_CounterClockwise };
public class CommandBlock : MonoBehaviour, IInteractable
{
    public InputType inputType;

    public bool IsTeleporting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact(Player player)
    {
        //Debug.Log("Interacted");
        player.Pickup(gameObject);
    }

    public void LookAt()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void LookAway()
    {
        GetComponent<Renderer>().material.color = Color.blue;
    }

}
