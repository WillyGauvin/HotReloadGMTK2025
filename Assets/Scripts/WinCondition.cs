using DG.Tweening;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public bool robotCanUse;
    public bool playerCanUse;

    [SerializeField] public bool isCompleted;

    public BoxCollider winConditionTrigger;
    public Transform flag;
    public float flagScaleAmount = 1.2f;

    private Tween flagAnimation = null;

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
                ActivateAnimation(true);
            }
        }
        if (other.TryGetComponent<RobotController>(out RobotController robot))
        {
            Player playerReff = FindAnyObjectByType<Player>();
            if (robotCanUse)
            {
                robot.HasReachedGoal(true);

                if (playerReff.isInBrain)
                {
                    LevelManager.instance.PlayerReachGoal(true);
                }
                LevelManager.instance.RobotReachGoal(true);
                ActivateAnimation(true);
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
                ActivateAnimation(false);
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
                ActivateAnimation(false);
            }
        }
    }

    private void ActivateAnimation(bool activate)
    {
        if (flag != null)
            if (activate)
            {
                flagAnimation = flag.transform.DOScale(flagScaleAmount, .5f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                flagAnimation.Kill();
                flag.transform.DOScale(1.0f, .2f);
                flagAnimation = null;
            }
    }
}
