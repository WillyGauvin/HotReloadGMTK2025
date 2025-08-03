using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool isLevelBeaten;
    [SerializeField] private bool willFullReset = false;

    public bool WillFullResest
    {
        get { return willFullReset; }
        set 
        {
            willFullReset = value;
            PlayerPrefs.SetInt("HardMode", Convert.ToInt32(willFullReset));
        }
    }
    [SerializeField] private bool hasBeatTutorial;
    [SerializeField] private bool failedLevel;

    [SerializeField] public bool isHoldingDownReset;
    [SerializeField] public float timeNeededToReset;
    [SerializeField] public float timeResetHeld;
    public static LevelManager instance { get; private set; }

    [SerializeField] private List<WinCondition> winConditionList;
    [SerializeField] public ResetRadialUI resetRadialUI;

    [SerializeField] public bool playerReachGoal;
    [SerializeField] public bool robotReachGoal;

    public UnityEvent OnNextLevelTransition;
    public UnityEvent OnRestartTransition;
    public BoxCollider waterTrigger;

    public ParticleSystem waterSplashParticle;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
            Debug.LogError("Found more than 1 LevelManager in scene");


        isLevelBeaten = false;
        playerReachGoal = false;
        robotReachGoal = false;

        WinCondition[] allWinConditions = FindObjectsByType<WinCondition>(FindObjectsSortMode.InstanceID);

        if (allWinConditions != null && allWinConditions.Length > 0)
        {
            foreach (WinCondition component in allWinConditions)
            {
                winConditionList.Add(component);
            }
        }
        else
        {
            Debug.LogError("No Win Condition Found, Be Sure To Add One");
        }

        timeNeededToReset = 1.5f;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waterTrigger != null)
            waterTrigger.enabled = true;

        int tempInt = PlayerPrefs.GetInt("HardMode", 0);

        if (tempInt != 0)
            willFullReset = true;
        else
            willFullReset = false;

        if (SceneManager.GetActiveScene().buildIndex >= 3)
        {
            hasBeatTutorial = true;
        }
    }

    private void Update()
    {
        if (isHoldingDownReset)
        {
            timeResetHeld += Time.deltaTime;

            // Show radial fill and update it
            if (resetRadialUI != null)
            {
                float fillAmount = timeResetHeld / timeNeededToReset;
                resetRadialUI.SetFillAmount(fillAmount);
                if (timeResetHeld >= timeNeededToReset)
                    resetRadialUI.Show();
            }
        }
        else
        {
            timeResetHeld = 0;
            if (resetRadialUI != null)
                resetRadialUI.Hide();

        }

        if (timeResetHeld >= timeNeededToReset)
        {
            StartCoroutine(ResetCurrentLevel());

            if (resetRadialUI != null)
                resetRadialUI.Hide();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(waterSplashParticle, other.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        if (other.GetComponent<RobotController>())
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.robot_splash);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.voice_fallWater);
        }
        else if (other.GetComponent<PlayerController>())
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.player_splash);
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.emotion_neutral);
            }
            else
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.emotion_ouch);
                RobotController.instance.SetExpression(RobotExpression.Ouch);
            }
        }
        else if (other.TryGetComponent<CommandBlock>(out CommandBlock commandBlock))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.player_splash);
            if (commandBlock.inputType == InputType.Pop)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.emotion_satisfied);
                RobotController.instance.SetExpression(RobotExpression.Happy);
            }

            else
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.emotion_angry);
                RobotController.instance.SetExpression(RobotExpression.Angry);
            }
        }

        if (other.TryGetComponent<Player>(out Player player) || other.TryGetComponent<RobotController>(out RobotController robot))
        {
            StartCoroutine(ResetCurrentLevel());
        }

        failedLevel = true;
    }

    void CheckCompletedLevel()
    {
        if (playerReachGoal && robotReachGoal)
        {
            AudioManager.instance.SetMusicArea(States_Music.level_finish);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.emotion_satisfied);
            RobotController.instance.SetExpression(RobotExpression.Happy);
            isLevelBeaten = true;
            StartCoroutine(PlayNextLevel());
        }
    }
    public void PlayerReachGoal(bool hasReachedGoal)
    {
        playerReachGoal = hasReachedGoal;
        CheckCompletedLevel();
    }
    public void RobotReachGoal(bool hasReachedGoal)
    {
        robotReachGoal = hasReachedGoal;
        CheckCompletedLevel();
    }

    private IEnumerator PlayNextLevel()
    {
        if (isLevelBeaten)
        {
            OnNextLevelTransition?.Invoke();

            yield return new WaitForSeconds(2);

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(currentSceneIndex + 1);

        }
    }

    public IEnumerator ResetCurrentLevel()
    {
        isLevelBeaten = false;

        AudioManager.instance.SetMusicArea(States_Music.level_fail);

        OnRestartTransition?.Invoke();

        yield return new WaitForSeconds(1.5f);

        if (willFullReset && failedLevel && hasBeatTutorial)
        {
            Debug.Log("hardmode worked");

            SceneManager.LoadScene(3);
        }
        else
        {
            Debug.Log("willFullReset: " + willFullReset + " failedLevel: " +  failedLevel + " hasBeatTutorial: " + hasBeatTutorial);
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}
