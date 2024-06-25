using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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
    private string playerName;
    private enum MessageTypeFilter
    {
        General,
        PlayerMessage,
        Info
    }
    private MessageTypeFilter currentFilter = MessageTypeFilter.General;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            /// If userData is null,
            /// it is possible that ApprovalCheck() in NetworkServer Not activated,
            /// the solution is to go to NetBootstrap scene
            /// and tick Connection Approval of NetworkManager to True.
            playerName = userData.userName;
        }

        SendMessageServerRpc("[System] Your join code is '" + PlayerPrefs.GetString("JoinCode") + "', You can use this code to invite friends.", Message.MessageType.info);
    }

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
        // Typing
        if (textInput.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageServerRpc(playerName + ": " + textInput.text, Message.MessageType.playerMessage);
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

    Color MessageTypeColor(Message.MessageType messageType)
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

    // Chat filter button
    // If you confuse thid code, This is a short version of how to write the method.
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
        info
    }
}