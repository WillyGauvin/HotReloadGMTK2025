using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        AMBIENCE,
        SFX,
    }

    [Header("Type")]
    [SerializeField] VolumeType volumeType;

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = AudioManager.instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioManager.instance.musicVolume;
                break;
            case VolumeType.AMBIENCE:
                volumeSlider.value = AudioManager.instance.ambienceVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.instance.SFXVolume;
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType.ToString());
                break;
        }
    }

    public void OnSliderValueChanged()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.instance.masterVolume = volumeSlider.value;
                break;

            case VolumeType.MUSIC:
                AudioManager.instance.musicVolume = volumeSlider.value;
                break;

            case VolumeType.AMBIENCE:
                AudioManager.instance.ambienceVolume = volumeSlider.value;
                break;

            case VolumeType.SFX:
                AudioManager.instance.SFXVolume = volumeSlider.value;
                break;

            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType.ToString());
                break;
        }
        AudioManager.instance.SaveToPlayerPrefs();

    }
}
