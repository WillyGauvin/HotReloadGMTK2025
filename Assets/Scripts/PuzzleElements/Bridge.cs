using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Bridge : MonoBehaviour
{
    Coroutine closeBridgeRef;
    Coroutine openBridgeRef;

    [SerializeField] GameObject[] leftBridge;
    [SerializeField] GameObject[] rightBridge;

    Vector3 leftClosedPos, rightClosedPos;

    bool isOpening,isClosing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftClosedPos = transform.position - new Vector3(0.5f * leftBridge.Length + 0.25f, 0.0f, 0.0f);
        rightClosedPos = transform.position + new Vector3(0.5f * leftBridge.Length + 0.25f, 0.0f, 0.0f);

        foreach (GameObject bridge in leftBridge)
        {
            bridge.transform.position = leftClosedPos;
        }
        foreach (GameObject bridge in rightBridge)
        {
            bridge.transform.position = rightClosedPos;
        }
    }

    public void OpenBridge()
    {
        openBridgeRef = StartCoroutine(CR_OpenBridge());
    }

    public void CloseBridge()
    {
        closeBridgeRef = StartCoroutine(CR_CloseBridge());
    }

    IEnumerator CR_OpenBridge()
    {
        isOpening = true;

        if (isClosing)
            StopCoroutine(closeBridgeRef);

        KillTweens();

        for (int i = 0; i < leftBridge.Length; i++)
        {
            leftBridge[i].transform.DOMove(transform.position + -transform.right * (-0.25f + 0.5f * (i + 1)), 1.0f);
            rightBridge[i].transform.DOMove(transform.position + transform.right * (-0.25f + 0.5f * (i + 1)), 1.0f);
        }

        yield return DOTween.Sequence().AppendInterval(1.0f);

        //Sounds

        isOpening = false;
    }

    IEnumerator CR_CloseBridge()
    {
        isClosing = true;

        if(isOpening)
            StopCoroutine(openBridgeRef);

        KillTweens();


        for (int i = 0; i < leftBridge.Length; i++)
        {
            leftBridge[i].transform.DOMove(leftClosedPos, 0.5f);
            rightBridge[i].transform.DOMove(rightClosedPos, 0.5f);
        }

        yield return DOTween.Sequence().AppendInterval(1.0f);

        //Sounds
        isClosing = false;
    }

    private void KillTweens()
    {
        foreach (GameObject gameObject in leftBridge)
        {
            gameObject.transform.DOKill();
        }
        foreach (GameObject gameObject in rightBridge)
        {
            gameObject.transform.DOKill();
        }
    }
}
