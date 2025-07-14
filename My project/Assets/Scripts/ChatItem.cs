using UnityEngine;
using UnityEngine.UI;

public class ChatItem : MonoBehaviour
{
    public Text senderNameText;
    public Text messageText;
    public Text timeText;
    public Image playerImage;
    public Image bubbleChat;
    public void SetInfo(string senderName, string message, string timeString, Color dogColor, bool isDead)
    {
        senderNameText.text = senderName;
        messageText.text = message;
        timeText.text = timeString;
        playerImage.color = dogColor;
        if (isDead)
            bubbleChat.color = new Color(1, 1, 1, 0.6f);
    }
}