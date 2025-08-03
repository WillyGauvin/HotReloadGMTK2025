using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public bool isInMenu = false;
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    private EventInstance snapShotInstance;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/music");
        ambienceBus = RuntimeManager.GetBus("bus:/ambience");
        sfxBus = RuntimeManager.GetBus("bus:/sfx");
    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.instance.ambience);
        InitializeMusic(isInMenu ? FMODEvents.instance.menuMusic : FMODEvents.instance.gameMusic);
        LoadPlayerPrefs();
        InitializeMuffle();
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
    }

    void LoadPlayerPrefs()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1.0f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);

        ambienceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    private void InitializeMuffle()
    {
        if (!InputReader.instance || !RobotController.instance || PlayerController.instance)
        {
            return;
        }
        float DistanceToBrain = Vector3.Distance(InputReader.instance.transform.position, PlayerController.instance.transform.position);
        float DistanceToWorld = Vector3.Distance(RobotController.instance.transform.position, PlayerController.instance.transform.position);

        if (DistanceToBrain < DistanceToWorld)
        {
            StartMuffle();
        }
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }
    public void SetMusicArea(States_Music area)
    {
        musicEventInstance.setParameterByName("level_music", (float)area);
    }

    public void StartMuffle()
    {
        snapShotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        snapShotInstance.release();
    }

    public void EndMuffle()
    {
        snapShotInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.muffle);
        snapShotInstance.start();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}