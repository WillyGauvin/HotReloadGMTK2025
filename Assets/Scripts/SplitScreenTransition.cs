using UnityEngine;
using UnityEngine.UI;
public class SplitScreenTransition : MonoBehaviour
{
    public static SplitScreenTransition instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Found another SplitScreenTransition");
        else
            instance = this;
    }

    Camera RobotCam;
    GameObject seperator;
// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] CameraTransition = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject obj in CameraTransition)
        {
            if (obj.TryGetComponent<Camera>(out RobotCam) && obj.name == "RobotCamera")
            {
                RobotCam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                Debug.Log("Robot cam found");
            }
            else if (obj.GetComponent<Image>())
            {
                seperator = obj;
                Debug.Log("Seperator found");

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
