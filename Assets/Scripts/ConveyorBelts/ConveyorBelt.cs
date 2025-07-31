using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public RobotInputs heltInput;
    public float raycastDrawDistance = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //if theres a hard coded input on it already, set the position of it
        if (heltInput != null)
        {
            heltInput.transform.position = GetItemPosition(0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Get the position of the belt and return the position with padding in the Y so the item is in the conveyor belt
    public Vector3 GetItemPosition(float padding)
    {
        Vector3 position = transform.position;

        return new Vector3(position.x, position.y + padding, position.z);
    }


    //Ray cast to find the belt infront of it and return it
    public ConveyorBelt FindNextBelt()
    {
        Transform currentBeltTransfomr = transform; ;
        RaycastHit hit;

        Vector3 forwardDir = transform.forward;

        Ray raySent = new Ray(currentBeltTransfomr.position, forwardDir);

        if (Physics.Raycast(raySent, out hit, raycastDrawDistance))
        {
            ConveyorBelt nextBelt = hit.collider.GetComponent<ConveyorBelt>();
            if (nextBelt != null)
            {
                return nextBelt;
            }

        }
        return null;
    }

    public void AddNewRobotInput(RobotInputs collidedInput)
    {
        if (collidedInput != null)
        {
            //If there is already one, ignore and dont change it (unless we want to make it bump it off)
            if (heltInput != null)
            {
                heltInput = collidedInput;
            }
        }
    }
}
