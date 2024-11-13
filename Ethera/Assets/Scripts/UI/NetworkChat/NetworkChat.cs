using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Text;

public class NetworkChat : NetworkBehaviour
{
	[Header("MessageText Settings")]
	[SerializeField] [Range(0, 100)] private int maxMessages = 25;
	[SerializeField] private GameObject messageTextPrefab, chatPanel;
	[SerializeField] private TMP_InputField textInput;
	[SerializeField] [Tooltip("Different types of text colors")] private Color playerMessage, info;
	
	[Header("Button")]
	[SerializeField] private Button _generalButton;
	[SerializeField] private Button _messageButton;
	[SerializeField] private Button _infoButton;
	[SerializeField] private Color activeButtonColor;
	[SerializeField] private Color inactiveButtonColor;
	
	private readonly List<Message> messageList = new();
	public string PlayerName {get; private set;}
	private enum MessageTypeFilter
	{
		General,
		PlayerMessage,
		Info,
	}
	private MessageTypeFilter currentFilter = MessageTypeFilter.General;
	[SerializeField] private GameObject player;
	
	#region OnNetworkSpawn & OnNetworkDespawn
	public override void OnNetworkSpawn()
	{
		SetPlayerName();
		SendFirstMessage();
		player = FindPlayerObject();
	}

	public override void OnNetworkDespawn()
	{
		SendMessageServerRpc("[System] " + PlayerName + " has left.", Message.MessageType.info);
	}
	#endregion
	#region Register & Unregister button click event, Update Button States
	private void OnEnable()
	{
		_generalButton.onClick.AddListener(OnClick_General);
		_messageButton.onClick.AddListener(OnClick_PlayerMessage);
		_infoButton.onClick.AddListener(OnClick_Info);
	}

	private void OnDisable()
	{
		_generalButton.onClick.RemoveListener(OnClick_General);
		_messageButton.onClick.RemoveListener(OnClick_PlayerMessage);
		_infoButton.onClick.RemoveListener(OnClick_Info);
	}

	private void UpdateButtonStates()
	{
		_generalButton.image.color = currentFilter == MessageTypeFilter.General ? activeButtonColor : inactiveButtonColor;
		_messageButton.image.color = currentFilter == MessageTypeFilter.PlayerMessage ? activeButtonColor : inactiveButtonColor;
		_infoButton.image.color = currentFilter == MessageTypeFilter.Info ? activeButtonColor : inactiveButtonColor;
	}
	#endregion
	#region Get Component & Detect typing
	private void Start()
	{
		UpdateButtonStates();
	}

	private void Update()
	{
		DetectTyping();
	}

	private void DetectTyping()
	{
		if (textInput.text != "")
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				SendMessageServerRpc(PlayerName + ": " + textInput.text, Message.MessageType.playerMessage);
				ShowBubbleText(textInput.text);
				textInput.text = "";
			}
		}
		else
		{
			if (!textInput.isFocused && Input.GetKeyDown(KeyCode.Return))
			{
				textInput.ActivateInputField();
			}
		}
	}
	#endregion
	#region Set PlayerName & MessageTypeColor & Send first message
	private void SetPlayerName()
	{
		if (IsServer)
		{
			UserData userData =
			HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
			
			if (userData != null)
			{
				PlayerName = userData.userName;
			}
			else
			{
				Debug.LogError("UserData is null. Ensure Connection Approval is enabled in the NetworkManager.");
			}
		}
		else
		{
			string payload = Encoding.UTF8.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);
			UserData userData = JsonUtility.FromJson<UserData>(payload);
			
			if (userData != null)
			{
				PlayerName = userData.userName;
			}
			else
			{
				Debug.LogError("Failed to deserialize UserData from payload.");
			}
		}
	}

	private Color MessageTypeColor(Message.MessageType messageType)
	{
		Color color = info;

		switch (messageType)
		{
			case Message.MessageType.playerMessage:
				color = playerMessage;
				break;

			case Message.MessageType.info:
				color = info;
				break;
		}

		return color;
	}
	
	private void SendFirstMessage()
	{
		if (IsServer)
		{
			SendMessageToChat("[System] Your join code is '" + HostSingleton.Instance.GameManager.JoinCode + "', You can use this code to invite friends.", Message.MessageType.info);
		}
		else
		{
			SendMessageToChat("[System] Your join code is '" + ClientSingleton.Instance.GameManager.JoinCode + "', You can use this code to invite friends.", Message.MessageType.info);
		}

		SendMessageServerRpc("[System] " + PlayerName + " has joined.", Message.MessageType.info);
	}
	#endregion
	#region Find PlayerObject by ClientId
	[System.Obsolete]
	private GameObject FindPlayerObject()
	{
		NetworkObject[] players = FindObjectsOfType<NetworkObject>();
		
		foreach (NetworkObject player in players)
		{
			if (player.OwnerClientId == NetworkManager.Singleton.LocalClientId && player.CompareTag("Player"))
			{
				return player.gameObject;
			}
		}
		return null;
	}
	#endregion
	#region Send Message
	[ServerRpc(RequireOwnership = false)]
	private void SendMessageServerRpc(string text, Message.MessageType messageType)
	{
		SendMessageClientRpc(text, messageType);
	}

	[ClientRpc]
	private void SendMessageClientRpc(string text, Message.MessageType messageType)
	{
		SendMessageToChat(text, messageType);
	}
	
	private void SendMessageToChat(string text, Message.MessageType messageType)
	{
		if (messageList.Count >= maxMessages)
		{
			Destroy(messageList[0].messageTextPrefab.gameObject);
			messageList.RemoveAt(0);
		}

		Message newMessage = new()
		{
			text = text,
			messageType = messageType
		};

		messageList.Add(newMessage);
		UpdateChatDisplay();
	}
    #endregion
    #region Send Bubble
    [System.Obsolete]
    private void ShowBubbleText(string text)
	{
		Debug.Log("ShowBubbleText() called here!");
		if (player == null)
		{
			player = FindPlayerObject();
		}
		else if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
		{
			Debug.Log("Here, bubble text should be called");
			playerHealth.ShowBubbleTextServerRpc(text);
		}
	}
	#endregion
	#region Chat Filter
	public void ChangeFilter(int filter)
	{
		currentFilter = (MessageTypeFilter)filter;
		UpdateChatDisplay();
		UpdateButtonStates();
	}

	private void UpdateChatDisplay()
	{
		foreach (Transform child in chatPanel.transform)
		{
			Destroy(child.gameObject);
		}

		IEnumerable<Message> filteredMessages = messageList;

		switch (currentFilter)
		{
			case MessageTypeFilter.General:
				default: filteredMessages = messageList;
				break;
			case MessageTypeFilter.PlayerMessage:
				filteredMessages = messageList.Where(m => m.messageType == Message.MessageType.playerMessage);
				break;
			case MessageTypeFilter.Info:
				filteredMessages = messageList.Where(m => m.messageType == Message.MessageType.info);
				break;
		}

		foreach (var message in filteredMessages)
		{
			GameObject newText = Instantiate(messageTextPrefab, chatPanel.transform);
			TMP_Text textComponent = newText.GetComponent<TMP_Text>();
			textComponent.text = message.text;
			textComponent.color = MessageTypeColor(message.messageType);
		}
	}

	public void OnClick_General() => ChangeFilter(0);
	public void OnClick_PlayerMessage() => ChangeFilter(1);
	public void OnClick_Info() => ChangeFilter(2);
	#endregion
}

[System.Serializable]
public class Message
{
	public string text;
	public TMP_Text messageTextPrefab;
	public MessageType messageType;

	public enum MessageType
	{
		playerMessage,
		info,
	}
}