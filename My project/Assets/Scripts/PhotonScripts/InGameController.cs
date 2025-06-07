using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    public GameObject playerPrefab;
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;
    PhotonView myPov;
    int whichPlayerIsCat;
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Vector2 randomPosition = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        myPov = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            PickCat();
        }
    }
    void PickCat()
    {
        whichPlayerIsCat = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        myPov.RPC("RPC_SyncCat", RpcTarget.All, whichPlayerIsCat);
    }
    [PunRPC]
    void RPC_SyncCat(int playerNumber)
    {
        whichPlayerIsCat = playerNumber;
        PlayerController.localPlayer.BecomeCat(whichPlayerIsCat);
    }
    public void OnClickCatWin()
    {
        PhotonNetwork.LoadLevel("5_EndGame");
    }
}
