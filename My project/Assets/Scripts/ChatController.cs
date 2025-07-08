using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChatController : MonoBehaviourPun
{
    [Serializable]
    public class ChatMessage
    {
        public string SenderName;
        public string MessageText;
        public string TimeString; // "HH:mm" format

        public ChatMessage(string senderName, string messageText, string timeString)
        {
            SenderName = senderName;
            MessageText = messageText;
            TimeString = timeString;
        }
    }

    private readonly List<ChatMessage> chatMessages = new List<ChatMessage>();
    public IReadOnlyList<ChatMessage> ChatMessages => chatMessages.AsReadOnly();
    public event Action OnChatMessagesUpdated; // UI can subscribe

    // Called by UI to send a message
    public void SendChatMessage(string messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText)) return;

        string senderName = PhotonNetwork.NickName;
        string timeString = DateTime.Now.ToString("HH:mm");

        // Add locally for sender
        AddChatMessage(senderName, messageText, timeString, true);

        // Send to others (UTC time for consistency, but display as local)
        photonView.RPC(nameof(RPC_ReceiveChatMessage), RpcTarget.Others, senderName, messageText, timeString);
    }

    [PunRPC]
    void RPC_ReceiveChatMessage(string senderName, string messageText, string timeString)
    {
        AddChatMessage(senderName, messageText, timeString, false);
    }

    private void AddChatMessage(string senderName, string messageText, string timeString, bool isLocalSender)
    {
        var chatMsg = new ChatMessage(senderName, messageText, timeString);
        chatMessages.Add(chatMsg);
        OnChatMessagesUpdated?.Invoke();
    }

    public IReadOnlyList<ChatMessage> GetChatMessages() => ChatMessages;

    public void ClearChat()
    {
        chatMessages.Clear();
        OnChatMessagesUpdated?.Invoke();
    }
}