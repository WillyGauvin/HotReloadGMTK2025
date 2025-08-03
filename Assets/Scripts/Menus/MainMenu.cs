using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private RectTransform levelSelectButtonParent;

    [SerializeField] private RectTransform menuSettings;
    [SerializeField] private RectTransform menuLevelselect;
    [SerializeField] private RectTransform menuCredits;
    [SerializeField] private RectTransform menuMain;

    [SerializeField] private Vector2 camOscMagnitude;
    [SerializeField] private Vector2 camOscSpeed;
    [SerializeField] private int firstLevelScene = 1;

    private Vector2 camInitialRotation;

    private void Start()
    {
        camInitialRotation = cam.rotation.eulerAngles;
        var levelCount = levelSelectButtonParent.childCount;
        for (var i = 0; i < levelCount; i++)
        {
            var iButCopiedBecauseCsharpLambdaCapturingIsWonky = i;
            levelSelectButtonParent.GetChild(i).GetComponent<Button>().onClick.AddListener(() => PlayLevel(iButCopiedBecauseCsharpLambdaCapturingIsWonky));
        }
    }

    private void Update()
    {
        cam.rotation = Quaternion.Euler(new Vector3(
            camInitialRotation.x + camOscMagnitude.x * Mathf.Sin(Time.time * camOscSpeed.x),
            camInitialRotation.y + camOscMagnitude.y * Mathf.Sin(Time.time * camOscSpeed.y),
            0f
        ));
    }

    public void GotoSettings()
    {
        if (!menuSettings)
        {
            return;
        }
        HideAllMenus();
        menuSettings.gameObject.SetActive(true);
    }

    public void GoToLevelselect()
    {
        if (!menuLevelselect)
        {
            return;
        }
        HideAllMenus();
        menuLevelselect.gameObject.SetActive(true);
    }

    public void GoToCredits()
    {
        if (!menuCredits)
        {
            return;
        }
        HideAllMenus();
        menuCredits.gameObject.SetActive(true);
    }

    public void GoBackToMain()
    {
        if (!menuMain)
        {
            return;
        }
        HideAllMenus();
        menuMain.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayLevel(int level)
    {
        SceneManager.LoadScene(firstLevelScene + level);
    }

    private void HideAllMenus()
    {
        menuSettings.gameObject.SetActive(false);
        menuLevelselect.gameObject.SetActive(false);
        menuCredits.gameObject.SetActive(false);
        menuMain.gameObject.SetActive(false);
    }
}
