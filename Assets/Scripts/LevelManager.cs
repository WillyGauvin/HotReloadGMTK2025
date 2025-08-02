using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool isLevelBeaten;
    public static LevelManager instance { get; private set; }

    [SerializeField] private List<WinCondition> winConditionList;

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

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waterTrigger != null)
            waterTrigger.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(waterSplashParticle, other.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        if (other.TryGetComponent<Player>(out Player player) || other.TryGetComponent<RobotController>(out RobotController robot))
        {
            StartCoroutine(ResetCurrentLevel());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerReachGoal && robotReachGoal)
        {
            AudioManager.instance.SetMusicArea(States_Music.level_finish);
            isLevelBeaten = true;
            StartCoroutine(PlayNextLevel());
        }
    }
    public void PlayerReachGoal(bool hasReachedGoal)
    {
        playerReachGoal = hasReachedGoal;
    }
    public void RobotReachGoal(bool hasReachedGoal)
    {
        robotReachGoal = hasReachedGoal;
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

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }
}
