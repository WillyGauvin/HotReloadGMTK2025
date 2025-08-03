using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private FMODUnity.EventReference sfxHover;
    [SerializeField] private FMODUnity.EventReference sfxClick;

    private bool isPointerOver = false;
    private float targetScale = 1f;

    private void Start()
    {
        text.raycastTarget = false;
    }

    private void OnDisable()
    {
        isPointerOver = false;
    }

    private void Update()
    {
        if (isPointerOver)
        {
            text.transform.localScale = Vector2.one * (targetScale + (1f - targetScale) * 0.5f * Mathf.Sin(Time.time * 6.28f));
            targetScale = Mathf.Lerp(targetScale, 1.2f, 0.2f);
        }
        else
        {
            text.transform.localScale = Vector2.one * targetScale;
            targetScale = Mathf.Lerp(targetScale, 1f, 0.2f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // targetScale = 2.5f;
        isPointerOver = false;
        FMODUnity.RuntimeManager.PlayOneShot(sfxClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        targetScale = 1.5f;
        FMODUnity.RuntimeManager.PlayOneShot(sfxHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }
}
