using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CommandBlockSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnPrefab;

    [SerializeField] Transform spawnTransform;

    [SerializeField] Transform placementTransform;

    GameObject myCommandBlock;

    Tween moveTween;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnCommandBlock());
    }

    private void Update()
    {
        if (myCommandBlock)
        {
            if (Vector3.Distance(myCommandBlock.transform.position, placementTransform.position) > 3.0f)
            {
                moveTween.Kill();
                myCommandBlock = null;
                StartCoroutine(SpawnCommandBlock());
            }
        }
    }

    IEnumerator SpawnCommandBlock()
    {
        yield return new WaitForSeconds(1.0f);

        myCommandBlock = Object.Instantiate(spawnPrefab, spawnTransform.position, spawnTransform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f));
        myCommandBlock.GetComponent<Rigidbody>().isKinematic = true;
        myCommandBlock.GetComponent<BoxCollider>().enabled = false;

        moveTween = myCommandBlock.transform.DOMoveY(placementTransform.position.y, 1.0f);
        yield return moveTween.WaitForCompletion();

        if (myCommandBlock)
        {
            myCommandBlock.GetComponent<Rigidbody>().isKinematic = false;
            myCommandBlock.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
