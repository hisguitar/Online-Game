using TMPro;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCode;

    private void Start()
    {
        joinCode.text = "Code\n" + PlayerPrefs.GetString("JoinCode");
    }
}