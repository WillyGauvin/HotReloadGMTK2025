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

    [field: Header("Death")]
    [field: SerializeField] public EventReference player_splash { get; private set; }
    [field: SerializeField] public EventReference robot_splash { get; private set; }
    [field: SerializeField] public EventReference robot_hitwall { get; private set; }

    [field: Header("DrawBridge")]
    [field: SerializeField] public EventReference drawBridge_lower { get; private set; }
    [field: SerializeField] public EventReference drawBridge_raise { get; private set; }

    [field: Header("InsideTank")]
    [field: SerializeField] public EventReference commandline_speedUp { get; private set; }
    [field: SerializeField] public EventReference commandline_slowDown { get; private set; }
    [field: SerializeField] public EventReference explosionBlock { get; private set; }


    [field: Header("Items")]
    [field: SerializeField] public EventReference block_spawner { get; private set; }
    [field: SerializeField] public EventReference gate_pop { get; private set; }
    [field: SerializeField] public EventReference item_drop { get; private set; }
    [field: SerializeField] public EventReference item_throw { get; private set; }
    [field: SerializeField] public EventReference key_collect { get; private set; }

    [field: Header("Menu")]
    [field: SerializeField] public EventReference menu_click { get; private set; }

    [field: Header("Player Movement")]
    [field: SerializeField] public EventReference player_footsteps_outside { get; private set; }
    [field: SerializeField] public EventReference player_footsteps_tank { get; private set; }
    [field: SerializeField] public EventReference player_tank_entry { get; private set; }
    [field: SerializeField] public EventReference player_tank_exit { get; private set; }

    [field: Header("Player Voice")]
    [field: SerializeField] public EventReference player_drop { get; private set; }
    [field: SerializeField] public EventReference player_action { get; private set; }
    [field: SerializeField] public EventReference player_pick_up { get; private set; }
    [field: SerializeField] public EventReference player_random_chatter { get; private set; }
    [field: SerializeField] public EventReference player_throw { get; private set; }
    [field: SerializeField] public EventReference player_walk { get; private set; }
    [field: SerializeField] public EventReference player_walkBox { get; private set; }

    [field: Header("Pressure Plate")]
    [field: SerializeField] public EventReference pressurePlate_pressed { get; private set; }
    [field: SerializeField] public EventReference pressurePlate_released { get; private set; }

    [field: Header("Tank Movement")]
    [field: SerializeField] public EventReference tank_driving { get; private set; }

    [field: Header("Tank Voices")]
    [field: SerializeField] public EventReference command_executed { get; private set; }
    [field: SerializeField] public EventReference command_instantly_executed { get; private set; }
    [field: SerializeField] public EventReference conversation_long { get; private set; }
    [field: SerializeField] public EventReference conversation_short { get; private set; }
    [field: SerializeField] public EventReference emotion_angry { get; private set; }
    [field: SerializeField] public EventReference emotion_neutral { get; private set; }
    [field: SerializeField] public EventReference emotion_ouch { get; private set; }
    [field: SerializeField] public EventReference emotion_satisfied { get; private set; }
    [field: SerializeField] public EventReference voice_driving { get; private set; }
    [field: SerializeField] public EventReference voice_fallWater { get; private set; }
    [field: SerializeField] public EventReference voice_wallHit { get; private set; }




    [field: Header("Snapshots")]
    [field: SerializeField] public EventReference muffle { get; private set; }




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