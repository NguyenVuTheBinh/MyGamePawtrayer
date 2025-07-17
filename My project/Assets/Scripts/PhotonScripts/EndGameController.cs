using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviourPunCallbacks
{
    public Text announceText;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_WinnerText", RpcTarget.All, WinnerAnnouncer.WinnerName);
        }
    }

    [PunRPC]
    void RPC_WinnerText(string winnerTeam)
    {
        announceText.text = winnerTeam;
    }
    
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("2_Lobby");
    }
    public void OnClickBackRoom()
    {
        PhotonNetwork.LoadLevel("3_PreGame");
    }
}
