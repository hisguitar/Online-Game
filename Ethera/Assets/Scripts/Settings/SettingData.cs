using UnityEngine;

[CreateAssetMenu(fileName = "SettingData", menuName = "ScriptableObject/Setting_Data", order = 0)]
public class SettingData : ScriptableObject
{
    [Header("Sound Setting Data")]
    [Range(0, 100)] public int musicVolume;
    [Range(0, 100)] public int SFXVolume;
}