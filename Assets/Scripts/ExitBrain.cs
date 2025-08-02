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
        else if (other.gameObject.TryGetComponent<CommandBlock>(out CommandBlock box))
        {
            if (!box.IsTeleporting)
            {
                StartCoroutine(MoveBox(box));
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
        player.transform.localScale = Vector3.one * 0.01f;

        player.transform.position = Robot.transform.position;
        player.transform.rotation = Robot.rotation * Quaternion.Euler(0, 180f, 0);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.player_TankExit, Robot.position);


        moveTween = player.transform.DOMove(Robot.position - Robot.forward * 2.0f, 0.5f);
        Tween scaleTween = player.transform.DOScale(1.0f, 0.5f);

        yield return moveTween.WaitForCompletion();

        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.isInBrain = false;
        player.IsControllable = true;
    }

    IEnumerator MoveBox(CommandBlock Box)
    {
        Box.IsTeleporting = true;
        Box.GetComponent<BoxCollider>().isTrigger = true;
        Box.GetComponent<Rigidbody>().isKinematic = true;

        Tween moveTween = Box.gameObject.transform.DOMove(transform.position, 1.0f);

        yield return moveTween.WaitForCompletion();

        Box.transform.localScale = Vector3.one * 0.01f;

        Box.transform.position = Robot.position;
        Box.transform.rotation = Robot.rotation;

        Tween scaleTween = Box.transform.DOScale(1.0f, 0.5f);

        Box.GetComponent<Rigidbody>().isKinematic = false;
        Box.GetComponent<BoxCollider>().isTrigger = false;

        Box.GetComponent<Rigidbody>().AddForce(-Robot.forward * 5.0f, ForceMode.Impulse);

        yield return new WaitForSeconds(2.0f);

        Box.IsTeleporting = false;
    }
}
