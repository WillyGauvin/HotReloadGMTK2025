using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public bool robotCanUse;
    public bool playerCanUse;

    [SerializeField] public bool isCompleted;

    public BoxCollider winConditionTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winConditionTrigger.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Tracked");

        if (other.TryGetComponent<Player>(out Player player))
        {
            if (playerCanUse)
            {
                LevelManager.instance.PlayerReachGoal(true);
            }
        }
        if (other.TryGetComponent<RobotController>(out RobotController robot))
        {
            Player playerReff = FindAnyObjectByType<Player>();
            if (robotCanUse)
            {
                if (playerReff.isInBrain)
                {
                    LevelManager.instance.PlayerReachGoal(true);
                }
                LevelManager.instance.RobotReachGoal(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.TryGetComponent<Player>(out Player player))
        {
            if (playerCanUse)
            {
                LevelManager.instance.PlayerReachGoal(false);
            }
        }
        if (other.TryGetComponent<RobotController>(out RobotController robot))
        {
            Player playerReff = FindAnyObjectByType<Player>();
            if (robotCanUse)
            {
                if (playerReff.isInBrain)
                {
                    LevelManager.instance.PlayerReachGoal(false);
                }
                LevelManager.instance.RobotReachGoal(false);
            }
        }
    }
}
