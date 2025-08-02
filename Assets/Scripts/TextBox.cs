using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private string currentText;
    [SerializeField] private BoxCollider currentTrigger;
    [SerializeField] private int currentIndex = 0;

    public GameObject speaker;
    public List<BoxCollider> allTriggers;
    public List<string> allText;

    private void Awake()
    {
        if (allTriggers.Count > 0)
        {
            foreach (var trigger in allTriggers)
            {
                trigger.enabled = false;
            }

            allTriggers[0].enabled = true;
            currentTrigger = allTriggers[0];
        }

        if (allText.Count > 0)
        {
            currentText = allText[0];
        }
        else
        {
            Debug.LogError("No Text Added to list of text boxes");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentTrigger != null)
        {
            if (other.TryGetComponent<Player>(out Player player) || other.TryGetComponent<RobotController>(out RobotController robot))
            {
                currentIndex++;

                //If There is a speaker, make it spawn from them into the middle
                if (speaker != null)
                {

                }
                //else just make the text box appear in the center of the screen
                else
                {

                }
            }
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void DisplayTextBox()
    {
        if (currentText != null)
        {
            //do thing ;)
        }
    }

    void HideTextBox()
    {
        //hide the text box
    }

    void DisplayNextTextBox()
    {
        if (allTriggers.Count > 0 && allText.Count > 1)
        {

        }
    }
}