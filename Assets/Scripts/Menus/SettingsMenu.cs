using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hardModeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHardModeClicked()
    {
        LevelManager.instance.WillFullResest = !LevelManager.instance.WillFullResest;

        SetText();
    }

    void SetText()
    {
        hardModeText.text = LevelManager.instance.WillFullResest ? "Enabled" : "Disabled";
        Debug.Log(LevelManager.instance.WillFullResest);
    }
}
