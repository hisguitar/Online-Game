using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [Header("Save Setting")]
    [SerializeField][Tooltip("Used for retrieve data only.")] private SettingData settingDataDefault;
    [SerializeField][Tooltip("Used for retrieve / save data.")] private SettingData settingDataPlayer;
    [SerializeField][Tooltip("Just put button to this blank, no need to attach function to button's OnClick()")] private Button resetButton;

    [Header("Sound Setting")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text SFXVolumeText;
    [SerializeField][Range(0, 100)] private float currentMusicVolume;
    [SerializeField][Range(0, 100)] private float currentSFXVolume;

    private enum MixerParameter
    {
        MusicVolume,
        SFXVolume
    }

    // OnEnable() will be called when the game object SetActive(true) / enabled = true
    private void OnEnable()
    {
        if (settingDataPlayer != null)
        {
            RetrievePlayerSetting();
        }

        resetButton.onClick.AddListener(RetrieveDefaultSetting);
        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        SFXSlider.onValueChanged.AddListener(UpdateSFXVolume);
    }

    // OnDisable() will be called when the game object SetActive(false) or enabled = false
    private void OnDisable()
    {
        resetButton.onClick.RemoveListener(RetrieveDefaultSetting);
        musicSlider.onValueChanged.RemoveListener(UpdateMusicVolume);
        SFXSlider.onValueChanged.RemoveListener(UpdateSFXVolume);
    }

    #region Update Volume
    private void UpdateMusicVolume(float value)
    {
        SetVolume(value, MixerParameter.MusicVolume, ref currentMusicVolume, musicVolumeText);
    }

    private void UpdateSFXVolume(float value)
    {
        SetVolume(value, MixerParameter.SFXVolume, ref currentSFXVolume, SFXVolumeText);
    }

    private void SetVolume(float value, MixerParameter mixerParameter, ref float currentVolume, TMP_Text volumeText)
    {
        currentVolume = value;
        if (mixerParameter == MixerParameter.MusicVolume)
        {
            settingDataPlayer.musicVolume = (int)value;
        }
        else if (mixerParameter == MixerParameter.SFXVolume)
        {
            settingDataPlayer.SFXVolume = (int)value;
        }

        float volume = value > 0 ? Mathf.Log10(value / 100f) * 20 : -80f;
        audioMixer.SetFloat(mixerParameter.ToString(), volume);
        volumeText.text = ((int)value).ToString();
    }
    #endregion

    #region Retrieve Setting Data
    private void RetrieveDefaultSetting()
    {
        RetrieveSetting(settingDataDefault);
    }

    private void RetrievePlayerSetting()
    {
        RetrieveSetting(settingDataPlayer);
    }

    private void RetrieveSetting(SettingData dataType)
    {
        SetVolume(dataType.musicVolume, MixerParameter.MusicVolume, ref currentMusicVolume, musicVolumeText);
        SetVolume(dataType.SFXVolume, MixerParameter.SFXVolume, ref currentSFXVolume, SFXVolumeText);
        UpdateUI();
    }
    #endregion

    #region Update UI
    private void UpdateUI()
    {
        audioMixer.GetFloat(MixerParameter.MusicVolume.ToString(), out float musicVolume);
        audioMixer.GetFloat(MixerParameter.SFXVolume.ToString(), out float SFXVolume);

        currentMusicVolume = Mathf.Pow(10, musicVolume / 20) * 100f;
        currentSFXVolume = Mathf.Pow(10, SFXVolume / 20) * 100f;

        musicSlider.value = currentMusicVolume;
        SFXSlider.value = currentSFXVolume;

        musicVolumeText.text = ((int)currentMusicVolume).ToString();
        SFXVolumeText.text = ((int)currentSFXVolume).ToString();
    }
    #endregion
}