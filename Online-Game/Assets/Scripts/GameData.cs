using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;

    private void Start()
    {
        joinCodeText.text = "Code\n" + PlayerPrefs.GetString("JoinCode");
    }
}