using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    public Text playerNameText;
    public Image playerImage;
    public Image outlineImage;
    [HideInInspector] public int playerId;

    public void Setup(string playerName, Color dogColor, bool isDead, int playerId)
    {
        this.playerId = playerId;
        
        if (playerNameText != null)
            playerNameText.text = playerName;
        if (playerImage != null)
        {
            playerImage.color = dogColor;
            playerImage.preserveAspect = true;
        }

        if (outlineImage != null)
            outlineImage.preserveAspect = true;
    }
}
