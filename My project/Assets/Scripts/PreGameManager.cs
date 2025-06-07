using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PreGameManager : MonoBehaviourPunCallbacks
{
    public Text roomName;

    private void Start()
    {
        roomName.text = "Room name:" + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("2_Lobby");
    }
}
