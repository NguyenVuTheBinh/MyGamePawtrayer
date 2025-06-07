using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text roomName;
    LobbyManager manager;

    private void Start()
    {
        manager = FindAnyObjectByType<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }
    public void OnCLickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
