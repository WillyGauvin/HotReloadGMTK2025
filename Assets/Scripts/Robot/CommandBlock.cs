using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum InputType { Forward, Rotate_Clockwise, Rotate_CounterClockwise, Pop };
public class CommandBlock : MonoBehaviour, IInteractable
{
    public InputType inputType;

    public bool IsTeleporting = false;


    public MeshRenderer mesh;
    public float highlightIntensity = 0.5f;
    public float highlightEaseTime = 0.5f;

    public bool IsTweening = false;

    public ConveyorBelt myBelt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact(Player player)
    {
        player.Pickup(this);
    }

    public void LookAt(Player player)
    {
        //GetComponent<Renderer>().material.color = Color.red;
        Highlight(highlightIntensity);
    }

    public void LookAway(Player player)
    {
        //GetComponent<Renderer>().material.color = Color.blue;
        Highlight(0.0f);
    }

    private void Highlight(float amount)
    {
        List<Material> mats = new List<Material>();
        mesh.GetMaterials(mats);
        foreach (Material mat in mats)
        {
            Tween tween = mat.DOFloat(amount, "_HighlightAmount", highlightEaseTime);
        }
    }

    public CommandBlock Pickup(Player player)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;

        StartCoroutine(MoveBlockToPlayer(player.PlayerCarryPosition));

        if (myBelt)
        {
            myBelt.HeldBox = null;
        }
        myBelt = null;

        return this;
    }

    public CommandBlock Pickup(ConveyorBelt Belt)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;

        StartCoroutine(MoveBlockToBelt(Belt.beltholdTransform));

        myBelt = Belt;

        return this;
    }

    IEnumerator MoveBlockToBelt(Transform target)
    {
        IsTweening = true;
        transform.SetParent(target);

        //transform.SetParent(null);
        Tween moveBlock = transform.DOMove(target.position, 0.5f);
        Tween rotateBlock = transform.DORotateQuaternion(target.rotation * Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f)), 0.5f);
        yield return moveBlock.WaitForCompletion();
        yield return rotateBlock.WaitForCompletion();

        IsTweening = false;

    }

    IEnumerator MoveBlockToPlayer(Transform target)
    {
        IsTweening = true;
        transform.SetParent(target);
        //transform.SetParent(null);

        Vector3 start = transform.position;
        float time = 0f;

        Tween rotateBlock = transform.DORotateQuaternion(target.rotation, 0.5f);

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            float t = time / 0.5f;

            Vector3 targetXZ = new Vector3(target.position.x, 0, target.position.z);
            Vector3 currentXZ = Vector3.Lerp(start, targetXZ, t);

            float y = Mathf.Sin(t * Mathf.PI) * 3.0f + Mathf.Lerp(start.y, target.position.y, t);

            transform.position = new Vector3(currentXZ.x, y, currentXZ.z);
            yield return null;
        }

        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);

        yield return rotateBlock.WaitForCompletion();

        IsTweening = false;
    }


}
