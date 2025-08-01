using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public void UnlockGate()
    {
        StartCoroutine(OpenGate());
    }

    IEnumerator OpenGate()
    {
        Vector3 originalPos = transform.position;
        Vector3 originalScale = transform.localScale;

        GetComponent<BoxCollider>().enabled = false;

        Tween raiseUp = transform.DOMoveY(2.0f, 2.0f);
        yield return raiseUp.WaitForPosition(0.5f);
        Tween pop = transform.DOScale(0.0f, 0.25f).SetEase(Ease.InBack, 2.0f);
        yield return pop.WaitForCompletion();
        yield return raiseUp.WaitForCompletion();

        yield return new WaitForSeconds(2.0f);

        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);

        //Destroy(this);
    }
}
