using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
	[SerializeField] private TMP_Text joinCodeText;

	[Header("Panel")]
	[SerializeField] private GameObject _gameHUDPanel;
	[SerializeField] private GameObject _storePanel;
	[SerializeField] private GameObject _rankPanel;
	[SerializeField] private GameObject _menuPanel;
	[SerializeField] private GameObject _insideMenuPanel;
	[SerializeField] private GameObject _optionsPanel;

	[Header("Button")]
	[SerializeField] private Button _storeButton;
	[SerializeField] private Button _rankButton;
	[SerializeField] private Button _menuButton;
	[SerializeField] private Button _resumeButton;
	[SerializeField] private Button _optionsButton;
	[SerializeField] private Button _mainMenuButton;
	[SerializeField] private Button _quitButton;
	[SerializeField] private Button _backToMenuButton;

	private void Start()
	{
		if (NetworkManager.Singleton.IsHost)
		{
			joinCodeText.text = "Code\n" + HostSingleton.Instance.GameManager.JoinCode;
		}
		else
		{
			joinCodeText.text = "Code\n" + ClientSingleton.Instance.GameManager.JoinCode;
		}
	}

	#region Register & Unregister button click event
	private void OnEnable()
	{
		_storeButton.onClick.AddListener(OnClick_Store);
		_rankButton.onClick.AddListener(OnClick_Rank);
		_menuButton.onClick.AddListener(OnClick_Menu);
		_resumeButton.onClick.AddListener(OnClick_Resume);
		_optionsButton.onClick.AddListener(OnClick_Options);
		_mainMenuButton.onClick.AddListener(OnClick_MainMenu);
		_quitButton.onClick.AddListener(OnClick_QuitGame);
		_backToMenuButton.onClick.AddListener(OnClick_BackToMenu);
	}

	private void OnDisable()
	{
		_storeButton.onClick.RemoveListener(OnClick_Store);
		_rankButton.onClick.RemoveListener(OnClick_Rank);
		_menuButton.onClick.RemoveListener(OnClick_Menu);
		_resumeButton.onClick.RemoveListener(OnClick_Resume);
		_optionsButton.onClick.RemoveListener(OnClick_Options);
		_mainMenuButton.onClick.RemoveListener(OnClick_MainMenu);
		_quitButton.onClick.RemoveListener(OnClick_QuitGame);
		_backToMenuButton.onClick.RemoveListener(OnClick_BackToMenu);
	}
	#endregion

	#region Button
	private void OnClick_Store()
	{
		if (_storePanel.activeSelf == false)
		{
			_storePanel.SetActive(true);
		}
		else if (_storePanel.activeSelf == true)
		{
			_storePanel.SetActive(false);
		}
	}
	
	private void OnClick_Rank()
	{
		if (_rankPanel.activeSelf == false)
		{
			_rankPanel.SetActive(true);
		}
		else if (_rankPanel.activeSelf == true)
		{
			_rankPanel.SetActive(false);
		}
	}

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
		_insideMenuPanel.SetActive(false);
		_optionsPanel.SetActive(true);
	}

	private void OnClick_BackToMenu()
	{
		_insideMenuPanel.SetActive(true);
		_optionsPanel.SetActive(false);
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