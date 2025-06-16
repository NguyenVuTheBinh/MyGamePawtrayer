using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    public Text playerNameText;
    public Image playerImage;
    [HideInInspector] public int playerId;

    public void Setup(string playerName, Sprite dogSprite, int playerId)
    {
        this.playerId = playerId;
        if (playerNameText != null)
            playerNameText.text = playerName;
        if (playerImage != null)
            playerImage.sprite = dogSprite;
    }
}
