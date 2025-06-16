using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MeetingPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject playerButtonPrefab; 
    public Transform buttonContainer; 
    public GameObject panel; 

    public void Show(List<PlayerInfo> players, System.Action<int> onVote)
    {
        panel.SetActive(true);

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (var player in players)
        {
            GameObject btnObj = Instantiate(playerButtonPrefab, buttonContainer);
            PlayerItem playerItem = btnObj.GetComponent<PlayerItem>();
            playerItem.Setup(player.playerName, player.dogSprite, player.playerId);

            btnObj.GetComponent<Button>().onClick.AddListener(() => {
                onVote?.Invoke(player.playerId); 
            });
        }
    }

    public void DisableVoting()
    {
        foreach (Transform child in buttonContainer)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null)
                btn.interactable = false;
        }
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
}