using UnityEngine;

[CreateAssetMenu(fileName = "SettingData", menuName = "ScriptableObject/Setting_Data", order = 0)]
public class SettingData : ScriptableObject
{
    [Header("Sound Setting Data")]
    public int musicVolume;
    public int SFXVolume;
}