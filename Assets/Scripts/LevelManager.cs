using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool isLevelBeaten;
    public static LevelManager instance { get; private set; }

    [SerializeField] private List<WinCondition> winConditionList;

    [SerializeField] private bool playerReachGoal;
    [SerializeField] private bool robotReachGoal;

    public Animator sceneTransition;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (playerReachGoal && robotReachGoal)
        {
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
            sceneTransition.SetTrigger("Start");

            yield return new WaitForSeconds(2);

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    public void ResetCurrentLevel()
    {
        isLevelBeaten = false;

        sceneTransition.SetTrigger("Start");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }
}
