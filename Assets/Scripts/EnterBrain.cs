using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EnterBrain : MonoBehaviour
{
    [SerializeField] Transform BrainEnterTransform;

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
        else if (other.gameObject.TryGetComponent<InteractableTest>(out InteractableTest box))
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

        Tween shrinkTween = player.gameObject.transform.DOScale(0.01f, 1.0f);
        Tween moveTween = player.gameObject.transform.DOMove(transform.position, 1.0f);

        yield return shrinkTween.WaitForCompletion();
        yield return moveTween.WaitForCompletion();

        player.transform.localScale = Vector3.one;

        player.transform.position = BrainEnterTransform.position;
        player.transform.rotation = BrainEnterTransform.rotation;

        Tween walkForward = player.transform.DOMove(BrainEnterTransform.position + BrainEnterTransform.forward * 5.0f, 0.5f);

        yield return walkForward.WaitForCompletion();

        player.GetComponent<CapsuleCollider>().isTrigger = false;
        player.IsControllable = true;
    }

    IEnumerator MoveBox(InteractableTest Box)
    {

        Box.IsTeleporting = true;
        Box.GetComponent<BoxCollider>().isTrigger = true;
        Box.GetComponent<Rigidbody>().isKinematic = true;

        Tween shrinkTween = Box.gameObject.transform.DOScale(0.01f, 1.0f);
        Tween moveTween = Box.gameObject.transform.DOMove(transform.position, 1.0f);

        yield return shrinkTween.WaitForCompletion();
        yield return moveTween.WaitForCompletion();

        Box.transform.localScale = Vector3.one;

        Box.transform.position = BrainEnterTransform.position;
        Box.transform.rotation = BrainEnterTransform.rotation;

        Box.GetComponent<Rigidbody>().isKinematic = false;
        Box.GetComponent<BoxCollider>().isTrigger = false;

        Box.GetComponent<Rigidbody>().AddForce(BrainEnterTransform.forward * 20.0f, ForceMode.Impulse);

        yield return new WaitForSeconds(2.0f);

        Box.IsTeleporting = false;
    }
}
