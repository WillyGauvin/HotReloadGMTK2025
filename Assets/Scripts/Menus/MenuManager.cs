using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Menu Manager in the scene.");
        }
        instance = this;
    }

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetMenus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetMenus()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        ResetMenus();
        pauseMenu.SetActive(true);
        PlayerController.instance.SwitchToActionMap(PlayerController.ActionMap.UI);
    }

    public void UnPause()
    {
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1.0f;
            ResetMenus();
            PlayerController.instance.SwitchToActionMap(PlayerController.ActionMap.Player);
        }
    }
}
