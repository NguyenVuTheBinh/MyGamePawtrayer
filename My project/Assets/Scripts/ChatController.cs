using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChatController : MonoBehaviourPun
{
    [Serializable]
    public class ChatMessage
    {
        public string senderName;
        public string messageText;
        public string timeString;
        public Color dogColor;
        public bool isDead;

        public ChatMessage(string senderName, string messageText, string timeString, Color dogColor, bool isDead)
        {
            this.senderName = senderName;
            this.messageText = messageText;
            this.timeString = timeString;
            this.dogColor = dogColor;
            this.isDead = isDead;
        }
    }

    public static ChatController Instance { get; private set; }
    public Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    private readonly List<ChatMessage> chatMessages = new List<ChatMessage>();
    public IReadOnlyList<ChatMessage> ChatMessages => chatMessages.AsReadOnly();
    public event Action OnChatMessagesUpdated;
    Color dogColor;
    bool isDead;

    private void Awake()
    {
        Instance = this;
    }
    public void SendChatMessage(string messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText)) return;

        string senderName = PhotonNetwork.NickName;
        string timeString = DateTime.Now.ToString("HH:mm");
        
        GameObject playerObj;
        if (playerObjects.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out playerObj))
        {
            SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
            dogColor = sr.color;
            isDead = playerObj.GetComponent<PlayerController>().isDead;
        }
        float colorR = dogColor.r;
        float colorG = dogColor.g;
        float colorB = dogColor.b;
        //send to others
        photonView.RPC("RPC_ReceiveChatMessage", RpcTarget.OthersBuffered, senderName, messageText, timeString, colorR, colorG, colorB, isDead);
        //add in local
        AddChatMessage(senderName, messageText, timeString, colorR, colorG, colorB, isDead, true);

    }

    [PunRPC]
    void RPC_ReceiveChatMessage(string senderName, string messageText, string timeString, float colorR, float colorG, float colorB, bool isDead)
    {
        AddChatMessage(senderName, messageText, timeString, colorR, colorG, colorB, isDead, false);
    }

    private void AddChatMessage(string senderName, string messageText, string timeString, float colorR, float colorG, float colorB, bool isDead, bool isLocalSender)
    {
        dogColor.r = colorR;
        dogColor.g = colorG;
        dogColor.b = colorB;
        var chatMsg = new ChatMessage(senderName, messageText, timeString, dogColor, isDead);
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