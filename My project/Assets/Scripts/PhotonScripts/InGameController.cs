using UnityEngine;
using Photon.Pun;

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
        if (PhotonNetwork.LocalPlayer.TagObject == null)
        {
            Vector2 randomPosition = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));
            GameObject playerObj = PhotonNetwork.Instantiate("Test_Player", randomPosition, Quaternion.identity);
            myPov = GetComponent<PhotonView>();
        }
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
