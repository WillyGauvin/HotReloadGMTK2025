using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class EnterBrain : MonoBehaviour
{
    [SerializeField] Transform BrainEnterTransform;
    [SerializeField] GameObject ParticleBurstPrefab;
    ConveyorBelt StartingBelt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartingBelt = InputReader.instance.initalBelt;
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
                if (LevelManager.instance.robotReachGoal)
                {
                    LevelManager.instance.playerReachGoal = true;
                }
            }
        }
        else if (other.gameObject.TryGetComponent<CommandBlock>(out CommandBlock box))
        {
            if (!box.IsTeleporting)
            {
                StartCoroutine(MoveBox(box));
                if (box.inputType != InputType.Pop)
                {
                    RobotController.instance.ReceiveInput(box.inputType, true);
                }
            }
        }
    }

    IEnumerator MovePlayer(Player player)
    {
        player.IsControllable = false;

        player.gameObject.transform.LookAt(new Vector3(transform.position.x, player.transform.position.y, transform.position.z));
        player.GetComponent<CapsuleCollider>().isTrigger = true;

        Tween shrinkTween = player.gameObject.transform.DOScale(0.01f, 1.0f);
        Tween moveTween = player.gameObject.transform.DOMove(transform.position, 1.0f);
        yield return moveTween.WaitForPosition(0.5f);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.player_tank_entry);

        yield return shrinkTween.WaitForCompletion();
        yield return moveTween.WaitForCompletion();

        AudioManager.instance.SetAmbienceParameter("ambience_transition", 0.0f);
        AudioManager.instance.StartMuffle();


        player.transform.localScale = Vector3.one;

        player.transform.position = BrainEnterTransform.position;
        player.transform.rotation = BrainEnterTransform.rotation;

        if (player.CarryObject)
        {
            if (player.CarryObject.inputType == InputType.Pop)
            {
                StartCoroutine(DoPop(player.CarryObject));
                player.CarryObject = null;
            }
        }

        Tween walkForward = player.transform.DOMove(BrainEnterTransform.position + BrainEnterTransform.forward * 5.0f, 0.5f);

        yield return walkForward.WaitForCompletion();

        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.isInBrain = true;
        player.IsControllable = true;
    }

    IEnumerator MoveBox(CommandBlock Box)
    {
        Box.IsTeleporting = true;
        Box.GetComponent<BoxCollider>().isTrigger = true;
        Box.GetComponent<Rigidbody>().isKinematic = true;

        Tween shrinkTween = Box.gameObject.transform.DOScale(0.01f, 0.5f);
        Tween moveTween = Box.gameObject.transform.DOMove(transform.position, 0.5f);

        yield return shrinkTween.WaitForCompletion();
        yield return moveTween.WaitForCompletion();

        Box.transform.localScale = Vector3.one;

        Box.transform.position = BrainEnterTransform.position;
        Box.transform.rotation = BrainEnterTransform.rotation;

        Box.GetComponent<Rigidbody>().isKinematic = false;
        Box.GetComponent<BoxCollider>().isTrigger = false;

        if (Box.inputType == InputType.Pop)
        {
            StartCoroutine(DoPop(Box));
        }
        else
        {
            ConveyorBelt currentBelt = StartingBelt;

            while (currentBelt.heldBox != null)
            {
                currentBelt = currentBelt.GetNextBelt();
                if (currentBelt == StartingBelt)
                {
                    currentBelt = null;
                    break;
                }
            }

            if (currentBelt)
            {
                Tween moveMove = Box.transform.DOMove(BrainEnterTransform.position + BrainEnterTransform.forward * 5.0f, 10.0f).SetSpeedBased(true);
                yield return moveMove.WaitForCompletion();
                currentBelt.PickupBox(Box, false);
            }
            else
            {
                Box.GetComponent<Rigidbody>().AddForce(BrainEnterTransform.forward * 20.0f, ForceMode.Impulse);
                yield return new WaitForSeconds(0.3f);
                //AudioManager.instance.PlayOneShot(FMODEvents.instance.item_throw);
            }

            yield return new WaitForSeconds(2.0f);

            Box.IsTeleporting = false;
        }
    }

    IEnumerator DoPop(CommandBlock Box)
    {
        Box.IsTeleporting = true;
        Box.transform.SetParent(null);
        Box.GetComponent<BoxCollider>().enabled = false;
        Box.GetComponent<Rigidbody>().isKinematic = true;
        Tween moveMove = Box.transform.DOMove(InputReader.instance.BrainCenter.position, 0.5f);
        Tween rotateBox = Box.transform.DORotateQuaternion(InputReader.instance.BrainCenter.rotation, 0.5f);
        yield return moveMove.WaitForCompletion();
        yield return rotateBox.WaitForCompletion();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.explosionBlock);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.emotion_ouch);
        RobotController.instance.SetExpression(RobotExpression.Ouch);
        Instantiate(ParticleBurstPrefab, Box.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        Tween explosion = Box.transform.DOPunchScale(new Vector3(2.0f, 2.0f, 2.0f), 0.5f);

        RobotController.instance.ReceiveInput(InputType.Pop, true);
        yield return explosion.WaitForCompletion();

        Destroy(Box.gameObject);

        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.emotion_angry);
        RobotController.instance.SetExpression(RobotExpression.Angry);
    }
}
