using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ExitBrain : MonoBehaviour
{
    [SerializeField] Transform Robot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            if (player.IsControllable)
            {
                StartCoroutine(MovePlayer(player));
            }
        }
    }

    IEnumerator MovePlayer(Player player)
    {
        player.IsControllable = false;

        player.gameObject.transform.LookAt(new Vector3(transform.position.x, player.transform.position.y, transform.position.z));

        player.GetComponent<CapsuleCollider>().isTrigger = true;

        Tween moveTween = player.transform.DOMove(transform.position, 1.0f);

        yield return moveTween.WaitForCompletion();

        player.transform.position = Robot.transform.position;
        player.transform.rotation = Robot.transform.rotation;

        moveTween = player.transform.DOMove(Robot.position + Robot.forward * 2.0f, 0.5f);

        yield return moveTween.WaitForCompletion();

        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.IsControllable = true;
    }
}
