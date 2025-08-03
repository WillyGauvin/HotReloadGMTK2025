using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Transform))]
public class TextBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private BoxCollider currentTrigger;

    [SerializeField] private string currentText;
    [SerializeField] private int maxTextBoxDisplayTime = 10;
    [SerializeField] private float charDelay = 0.04f;
    [SerializeField] private float showDelay = 3f;

    [SerializeField] private TextBox nextTextBox;
    [SerializeField] private float delayBeforeNextBox = 0.5f;

    [SerializeField] private GameObject speaker;
    private static GameObject sharedSpeaker;

    private Coroutine activeCoroutine;

    private void Awake()
    {
        if (textMesh == null)
            Debug.LogError("TextMeshPro reference missing!");

        if (currentTrigger != null)
        {
            currentTrigger.enabled = true;
        }
        else
        {
            Debug.Log("No trigger assigned — textbox will auto-show after delay.");
        }
    }

    private void Start()
    {
        if (speaker != null)
        {
            sharedSpeaker = speaker;
        }
        else if (sharedSpeaker != null)
        {
            speaker = sharedSpeaker;
        }

        if (speaker != null)
        {
            Vector3 newPosition = speaker.transform.position;
            transform.position = new Vector3(newPosition.x, newPosition.y + 6f, newPosition.z);
        }

        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);

        if (currentTrigger == null)
        {
            gameObject.SetActive(true);
            Invoke(nameof(ShowAutomatically), showDelay);
        }
    }

    private void ShowAutomatically()
    {
        TriggerDisplay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentTrigger == null) return;

        if (other.TryGetComponent(out Player _) || other.TryGetComponent(out RobotController _))
        {
            TriggerDisplay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentTrigger == null) return;

        if (other.TryGetComponent(out Player _) || other.TryGetComponent(out RobotController _))
        {
            HideTextBox(); // only hide on exit, no coroutine canceling
        }
    }

    public void TriggerDisplay()
    {
        if (activeCoroutine == null)
        {
            activeCoroutine = StartCoroutine(DisplayTextBox());
        }
    }

    IEnumerator DisplayTextBox()
    {
        textMesh.text = "";
        gameObject.SetActive(true);
        textMesh.ForceMeshUpdate();

        transform.localScale = Vector3.zero;
        yield return transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

        for (int i = 0; i < currentText.Length; i++)
        {
            textMesh.text += currentText[i];
            textMesh.ForceMeshUpdate();
            yield return new WaitForSeconds(charDelay);
        }

        yield return new WaitForSeconds(maxTextBoxDisplayTime);

        yield return HideTextBox();

        if (nextTextBox != null)
        {
            yield return new WaitForSeconds(delayBeforeNextBox);
            nextTextBox.TriggerDisplay();
        }

        activeCoroutine = null;
    }

    IEnumerator HideTextBox()
    {
        yield return transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
        gameObject.SetActive(false);
    }
}
