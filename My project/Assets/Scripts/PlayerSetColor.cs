using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerSetColor : MonoBehaviourPunCallbacks
{
    Color color1 = new Color32(214, 29, 29, 255);
    Color color2 = new Color32(200, 48, 183, 255);
    Color color3 = new Color32(40, 50, 180, 255);
    Color color4 = new Color32(60, 195, 195, 255);
    Color color5 = new Color32(10, 171, 1, 255);
    Color color6 = new Color32(240, 163, 32, 255);
    Color color7 = new Color32(50, 50, 50, 255);
    Color color8 = new Color32(188, 188, 188, 255);
    Color color9 = new Color32(255, 100, 0, 255);
    public ColorBlock buttonStatusColor;

    public Color[] availableColors;
    public Button[] colorButtons;
    public Color fadedColorFactor = new Color(0, 0, 0, 0.6f); // For unavailable buttons

    private int myColorIndex = -1;
    private const string COLOR_KEY_PREFIX = "color_";
    [SerializeField] GameObject colorPanel;
    private void Awake()
    {
        availableColors = new Color[] {color1, color2, color3, color4, color5, color6, color7, color8, color9};
    }
    void Start()
    {
        // Setup button listeners
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int idx = i;
            colorButtons[i].onClick.AddListener(() => OnColorButtonClicked(idx));
        }

        // Try to claim a color on join
        if (PhotonNetwork.InRoom)
        {
            AssignRandomColor();
        }
    }

    void AssignRandomColor()
    {
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < availableColors.Length; i++)
        {
            object owner;
            if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(COLOR_KEY_PREFIX + i, out owner) || owner == null)
                freeIndices.Add(i);
        }
        if (freeIndices.Count == 0) return; // All colors taken

        int chosenIdx = freeIndices[Random.Range(0, freeIndices.Count)];
        TryClaimColor(chosenIdx);
    }

    void TryClaimColor(int colorIdx)
    {
        // Release old color if any
        if (myColorIndex != -1)
            ReleaseColor(myColorIndex);

        // Try to claim new color atomically
        Hashtable props = new Hashtable();
        props[COLOR_KEY_PREFIX + colorIdx] = PhotonNetwork.LocalPlayer.ActorNumber;
        Hashtable expected = new Hashtable();
        expected[COLOR_KEY_PREFIX + colorIdx] = null;

        if (PhotonNetwork.CurrentRoom.SetCustomProperties(props, expected))
        {
            myColorIndex = colorIdx;
            UpdatePlayerColor(colorIdx);
        }
        else
        {
            // Failed to claim (someone else got it), try another or show error
            AssignRandomColor();
        }
    }

    void ReleaseColor(int colorIdx)
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;
        // Only release if we actually own it
        object owner;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(COLOR_KEY_PREFIX + colorIdx, out owner)
            && (int)owner == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Hashtable props = new Hashtable();
            props[COLOR_KEY_PREFIX + colorIdx] = null;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    public void OnColorButtonClicked(int colorIdx)
    {
        // Check if that color is available
        object owner;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(COLOR_KEY_PREFIX + colorIdx, out owner) && owner != null)
        {
            // Not available
            return;
        }
        TryClaimColor(colorIdx);
    }

    void UpdatePlayerColor(int colorIdx)
    {
        Color32 chosenColor = availableColors[colorIdx];
        ExitGames.Client.Photon.Hashtable colorProp = new ExitGames.Client.Photon.Hashtable
        {
            { "colorR", chosenColor.r },
            { "colorG", chosenColor.g },
            { "colorB", chosenColor.b },
            { "colorA", chosenColor.a }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(colorProp);
        PlayerController.localPlayer.SetColor(availableColors[colorIdx]);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // Update button visuals based on current room color ownership
        for (int i = 0; i < colorButtons.Length; i++)
        {
            object owner;
            bool taken = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(COLOR_KEY_PREFIX + i, out owner) && owner != null;
            var img = colorButtons[i].image;
            img.color = availableColors[i];
            colorButtons[i].interactable = !taken;

            if (taken)
                img.color = Color.Lerp(availableColors[i], fadedColorFactor, 0.6f); // faded
        }
    }

    public override void OnLeftRoom()
    {
        // Release color when player leaves
        if (myColorIndex != -1)
            ReleaseColor(myColorIndex);
        myColorIndex = -1;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Clean up: if another player leaves, their color becomes available (handled by release logic)
        OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public void ButtonOrderPanelClose()
    {
        colorPanel.SetActive(false);
    }
}
