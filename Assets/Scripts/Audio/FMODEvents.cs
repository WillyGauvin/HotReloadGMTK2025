using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference gameMusic { get; private set; }
    [field: SerializeField] public EventReference menuMusic { get; private set; }

    [field: Header("Item")]
    [field: SerializeField] public EventReference item { get; private set; }

    [field: Header("Player")]
    [field: SerializeField] public EventReference player_Actions { get; private set; }
    [field: SerializeField] public EventReference player_Footsteps { get; private set; }
    [field: SerializeField] public EventReference player_TankEntry { get; private set; }
    [field: SerializeField] public EventReference player_TankExit { get; private set; }

    [field: Header("Tank")]
    [field: SerializeField] public EventReference tank_Driving { get; private set; }
    [field: SerializeField] public EventReference tank_Commands { get; private set; }

    [field: Header("Puzzle Elements")]
    [field: SerializeField] public EventReference  pressurePlate{ get; private set; }
    [field: SerializeField] public EventReference  drawBridge{ get; private set; }

    [field: Header("Menu")]
    [field: SerializeField] public EventReference menuClick { get; private set; }




    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}