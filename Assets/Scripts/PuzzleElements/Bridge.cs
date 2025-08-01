using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Bridge : MonoBehaviour
{
    Coroutine closeBridgeRef;
    Coroutine openBridgeRef;

    [SerializeField] GameObject leftBridge;
    [SerializeField] GameObject rightBridge;

    [SerializeField] GameObject sideBridge;
    BoxCollider[] bridgeColliders;

    float rightClosedPosX = 1.5f;
    float leftClosedPosX = -1.5f;

    bool isOpening,isClosing;

    private HashSet<GameObject> validObjectsInTrigger = new HashSet<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftBridge.transform.localPosition = new Vector3(leftClosedPosX, 0.0f, 0.0f);
        rightBridge.transform.localPosition = new Vector3(rightClosedPosX, 0.0f, 0.0f);

        bridgeColliders = sideBridge.GetComponents<BoxCollider>();

        foreach (BoxCollider collider in bridgeColliders)
        {
            collider.enabled = false;
        }
    }

    private void OpenBridge()
    {
        openBridgeRef = StartCoroutine(CR_OpenBridge());
    }

    private void CloseBridge()
    {
        closeBridgeRef = StartCoroutine(CR_CloseBridge());
    }

    public void SteppedOn(Collider other)
    {
        if (IsValidObject(other))
        {
            validObjectsInTrigger.Add(other.gameObject);

            if (validObjectsInTrigger.Count == 1)
            {
                OpenBridge();
            }
        }
    }

    public void SteppedOff(Collider other)
    {
        if (IsValidObject(other))
        {
            validObjectsInTrigger.Remove(other.gameObject);

            if (validObjectsInTrigger.Count == 0)
            {
                CloseBridge();
            }
        }
    }

    private bool IsValidObject(Collider collider)
    {
        return collider.GetComponent<Player>() || collider.GetComponent<RobotController>();
    }

    IEnumerator CR_OpenBridge()
    {
        foreach (BoxCollider collider in bridgeColliders)
        {
            collider.enabled = true;
        }
        isOpening = true;

        if (isClosing)
            StopCoroutine(closeBridgeRef);

        leftBridge.transform.DOKill();
        rightBridge.transform.DOKill();

        Tween leftMove = leftBridge.transform.DOLocalMoveX(0.0f, 1.0f);
        Tween rightMove = rightBridge.transform.DOLocalMoveX(0.0f, 1.0f);

        yield return leftMove.WaitForCompletion();
        yield return rightMove.WaitForCompletion();
        //Sounds

        isOpening = false;
    }

    IEnumerator CR_CloseBridge()
    {
        isClosing = true;

        foreach (BoxCollider collider in bridgeColliders)
        {
            collider.enabled = false;
        }

        if (isOpening)
            StopCoroutine(openBridgeRef);

        leftBridge.transform.DOKill();
        rightBridge.transform.DOKill();

        Tween leftMove = leftBridge.transform.DOLocalMoveX(leftClosedPosX, 1.0f);
        Tween rightMove = rightBridge.transform.DOLocalMoveX(rightClosedPosX, 1.0f);

        yield return leftMove.WaitForCompletion();
        yield return rightMove.WaitForCompletion();

        //Sounds
        isClosing = false;
    }


}
