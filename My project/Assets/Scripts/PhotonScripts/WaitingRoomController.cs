using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class WaitingRoomController : MonoBehaviour
{
    PhotonView myPov;
    [SerializeField] float timeToStart;
    float timer;
    bool readyToStart;
    [SerializeField] GameObject startButton;
    [SerializeField] Text countDown;
    [SerializeField] int nextLevel;

    private void Start()
    {
        myPov = GetComponent<PhotonView>();
        timer = timeToStart;
    }
    private void Update()
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);

        if (readyToStart)
        {
            timer -= Time.deltaTime;
            countDown.text = ((int)timer).ToString();
        }
        else
        {
            timer = timeToStart;
            countDown.text = "";
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (timer <= 0)
            {
                timer = 100;
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel(nextLevel);
            }
        }
    }
    public void Play()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            myPov.RPC("RPC_Play", RpcTarget.All);
        }
    }
    [PunRPC]
    void RPC_Play()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        readyToStart = !readyToStart;
    }
}
