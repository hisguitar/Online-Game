using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NetworkChat : NetworkBehaviour
{
    private string playerName;

    public int maxMessages = 25;
    public GameObject messageTextPrefab, chatPanel;
    [SerializeField] private TMP_InputField textInput;
    public Color playerMessage, info;

    [SerializeField] List<Message> messageList = new();

    private void Start()
    {
        playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Player");
    }

    private void Update()
    {
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

        // Press 'Spacebar' to test info message
        if (!textInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SendMessageServerRpc("You pressed the space key!", Message.MessageType.info);
                Debug.Log("Space");
            }
        }
    }

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
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new();
        newMessage.text = text;
        GameObject newText = Instantiate(messageTextPrefab, chatPanel.transform);
        newMessage.messageTextPrefab = newText.GetComponent<TMP_Text>();
        newMessage.messageTextPrefab.text = newMessage.text;
        newMessage.messageTextPrefab.color = MessageTypeColor(messageType);
        messageList.Add(newMessage);
    }

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

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
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