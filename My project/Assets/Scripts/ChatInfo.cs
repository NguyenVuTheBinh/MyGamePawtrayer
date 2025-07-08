using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChatInfo : MonoBehaviour
{
    [Header("UI References")]
    public GameObject chatPanel;
    public Button chatButton;
    public InputField chatInputField;
    public Button sendButton;
    public RectTransform chatContentParent;
    public GameObject chatItemPrefab;

    private ChatController chatController;

    void Start()
    {
        chatController = FindFirstObjectByType<ChatController>();
        chatPanel.SetActive(false);

        chatButton.onClick.AddListener(ToggleChatPanel);
        sendButton.onClick.AddListener(OnSendButtonClicked);
        chatController.OnChatMessagesUpdated += UpdateChatUI;
    }

    void ToggleChatPanel()
    {
        chatPanel.SetActive(!chatPanel.activeSelf);
        var allControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        var myPlayerController = allControllers.FirstOrDefault(p => p.myPov.IsMine);
        if (chatPanel.activeSelf)
        {
            UpdateChatUI();
            myPlayerController.DisableInputActions();
        }
        else
        {
            myPlayerController.EnableInputActions();
        }
    }

    void OnSendButtonClicked()
    {
        if (!string.IsNullOrWhiteSpace(chatInputField.text))
        {
            chatController.SendChatMessage(chatInputField.text);
            chatInputField.text = "";
        }
    }

    void UpdateChatUI()
    {
        // Clear previous chat items
        foreach (Transform child in chatContentParent)
            Destroy(child.gameObject);

        var messages = chatController.GetChatMessages();
        foreach (var msg in messages)
        {
            var go = Instantiate(chatItemPrefab, chatContentParent);
            var chatItem = go.GetComponent<ChatItem>();
            chatItem.SetInfo(msg.SenderName, msg.MessageText, msg.TimeString);
        }
    }

    void OnDestroy()
    {
        chatController.OnChatMessagesUpdated -= UpdateChatUI;
    }
}