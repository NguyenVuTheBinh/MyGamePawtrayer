using UnityEngine;
using UnityEngine.UI;

public class ChatItem : MonoBehaviour
{
    public Text senderNameText;
    public Text messageText;
    public Text timeText;

    public void SetInfo(string senderName, string message, string timeString)
    {
        senderNameText.text = senderName;
        messageText.text = message;
        timeText.text = timeString;
    }
}