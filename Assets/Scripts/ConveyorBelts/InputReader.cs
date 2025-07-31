using System;
using System.Linq;
using UnityEngine;

public enum InputType { Up, Down, Left, Right };

public class InputReader : MonoBehaviour
{
    public GameObject inputReader;

    public InputType inputType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        inputReader = gameObject;

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
