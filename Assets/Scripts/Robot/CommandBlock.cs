using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum InputType { Forward, Rotate_Clockwise, Rotate_CounterClockwise };
public class CommandBlock : MonoBehaviour, IInteractable
{
    public InputType inputType;

    public bool IsTeleporting = false;

    public MeshRenderer mesh;
    public float highlightIntensity = 0.5f;
    public float highlightEaseTime = 0.5f;

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
        Debug.Log("Interacted with Block");
        player.Pickup(this);
    }

    public void LookAt()
    {
        //GetComponent<Renderer>().material.color = Color.red;
        Highlight(highlightIntensity);
    }

    public void LookAway()
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
}
