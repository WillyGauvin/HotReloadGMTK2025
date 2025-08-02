using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private Gate myGate;
    [SerializeField] private GameObject keyMesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (myGate == null)
        {
            Debug.LogError("Key does not have a Gate set to it");
        }
        StartCoroutine(KeyBob());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() || other.gameObject.GetComponent<RobotController>())
        {
            StartCoroutine(CollectKey());
        }
    }

    IEnumerator CollectKey()
    {
        Vector3 OriginalPosition = transform.position;

        AudioManager.instance.PlayOneShot(FMODEvents.instance.key_collect);
        Tween rotation = transform.DORotate(new Vector3(0.0f, 2880.0f, 0.0f), 2.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        Tween moveToGate = transform.DOJump(myGate.transform.position, 3.0f, 2, 1.5f);

        yield return moveToGate.WaitForCompletion();
        yield return rotation.WaitForCompletion();

        if (myGate)
        {
            myGate.UnlockGate();
        }

        Tween pop = transform.DOScale(0.0f, 0.25f).SetEase(Ease.InBack, 2.0f);
        yield return pop.WaitForCompletion();

        Destroy(gameObject);

        //transform.position = OriginalPosition;
    }

    IEnumerator KeyBob()
    {
        keyMesh.transform.position = new Vector3(keyMesh.transform.position.x, 0.25f, keyMesh.transform.position.z);
        while (true)
        {
            Tween bobTween = keyMesh.transform.DOLocalMoveY(0.75f, 1.0f).SetEase(Ease.InOutQuad);
            yield return bobTween.WaitForCompletion();
            bobTween = keyMesh.transform.DOLocalMoveY(0.25f, 1.0f).SetEase(Ease.InOutQuad);
            yield return bobTween.WaitForCompletion();
        }
    }
}
