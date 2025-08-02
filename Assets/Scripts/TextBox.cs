using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private string currentText;
    [SerializeField] private BoxCollider currentTrigger;
    [SerializeField] private int currentTextIndex = 0;
    [SerializeField] private int currentTriggerIndex = 0;
    [SerializeField] private int maxTextBoxDisplayTime = 0;

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

                DisplayNextTextBox();

            }
        }
    }
    void Start()
    {
        currentTriggerIndex = 0;
        currentTextIndex = 0;
        StartCoroutine(DisplayTextBox());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DisplayTextBox()
    {
        if (currentText != null)
        {
            // If There is a speaker, make it spawn from them into the middle
            if (speaker)
            {

                //do thing :)
                yield return new WaitForSeconds(maxTextBoxDisplayTime);
            }
            else
            {

                //do thing :)
                yield return new WaitForSeconds(maxTextBoxDisplayTime);
            }
        }
    }

    void HideTextBox()
    {
        //hide the text box canvas
    }

    void DisplayNextTextBox()
    {
        if (allTriggers.Count > 0 && allText.Count > 0)
        {
            if (currentTriggerIndex < allTriggers.Count - 1)
            {
                HideTextBox();
            }
            else
            {
                currentText = allText[currentTextIndex];
                currentTrigger = allTriggers[currentTriggerIndex];
            }
            StartCoroutine(DisplayTextBox());
            currentTriggerIndex++;
            currentTextIndex++;
        }
    }
}