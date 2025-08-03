using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Finale : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void PlayAnimation()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutCubic);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
