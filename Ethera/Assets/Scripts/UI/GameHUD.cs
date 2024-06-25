using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;

    [Header("Panel")]
    [SerializeField] private GameObject _gameHUDPanel;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _optionsPanel;

    [Header("Button")]
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        joinCodeText.text = "Code\n" + HostSingleton.Instance.GameManager.JoinCode;
    }

    private void OnEnable()
    {
        _menuButton.onClick.AddListener(OnClick_Menu);
        _resumeButton.onClick.AddListener(OnClick_Resume);
        _optionsButton.onClick.AddListener(OnClick_Options);
        _mainMenuButton.onClick.AddListener(OnClick_MainMenu);
        _quitButton.onClick.AddListener(OnClick_QuitGame);
    }

    private void OnDisable()
    {
        _menuButton.onClick.RemoveListener(OnClick_Menu);
        _resumeButton.onClick.RemoveListener(OnClick_Resume);
        _optionsButton.onClick.RemoveListener(OnClick_Options);
        _mainMenuButton.onClick.RemoveListener(OnClick_MainMenu);
        _quitButton.onClick.RemoveListener(OnClick_QuitGame);
    }

    #region Button
    private void OnClick_Menu()
    {
        _gameHUDPanel.SetActive(false);
        _menuPanel.SetActive(true);
    }

    private void OnClick_Resume()
    {
        _gameHUDPanel.SetActive(true);
        _menuPanel.SetActive(false);
    }

    private void OnClick_Options()
    {
        _menuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
    }

    private void OnClick_MainMenu()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown();
        }

        ClientSingleton.Instance.GameManager.Disconnect();
    }

    private void OnClick_QuitGame()
    {
        Application.Quit();
    }
    #endregion
}