using UnityEngine;
using UnityEngine.UI;

public class ResetRadialUI : MonoBehaviour
{
    [SerializeField] private Image radialImage;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void SetFillAmount(float amount)
    {
        if (radialImage != null)
            radialImage.fillAmount = Mathf.Clamp01(amount);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        SetFillAmount(0);
    }
}