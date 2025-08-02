using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;

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

    private EventInstance bridgeRaise;
    private EventInstance bridgeLower;

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

        bridgeRaise = AudioManager.instance.CreateInstance(FMODEvents.instance.drawBridge_raise);
        bridgeLower = AudioManager.instance.CreateInstance(FMODEvents.instance.drawBridge_lower);

        //bridgeSound = AudioManager.instance.CreateInstance(FMODEvents.instance.drawBridge);
        //FMOD.ATTRIBUTES_3D attributes = FMODUnity.RuntimeUtils.To3DAttributes(gameObject);
        //bridgeSound.set3DAttributes(attributes);
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
            AudioManager.instance.PlayOneShot(FMODEvents.instance.pressurePlate_pressed);
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
            AudioManager.instance.PlayOneShot(FMODEvents.instance.pressurePlate_released);
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
        bridgeRaise.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bridgeLower.start();

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
        ////bridgeSound.setParameterByName("bridge_state", 1);
        ////bridgeSound.start();
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.drawBridge_raise);
        bridgeLower.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        bridgeRaise.start();

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
