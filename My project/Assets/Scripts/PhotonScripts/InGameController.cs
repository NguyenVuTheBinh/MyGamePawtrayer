using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UI;
using System.Linq;

public class InGameController : MonoBehaviour
{
    //what to spawn
    public GameObject playerPrefab;
    //spawn place
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;
    public PhotonView myPov;
    //role
    int whichPlayerIsCat;
    //just button
    public GameObject catWinButton;
    //for meeting
    public MeetingPanel meetingPanel;
    public Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    public static InGameController Instance { get; private set; }
    private Dictionary<int, int> votes = new Dictionary<int, int>();
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.TagObject == null)
        {
            Vector2 randomPosition = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));
            GameObject playerObj = PhotonNetwork.Instantiate("Test_Player", randomPosition, Quaternion.identity);
            playerObjects[playerObj.GetComponent<PhotonView>().Owner.ActorNumber] = playerObj;
            myPov = GetComponent<PhotonView>();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            PickCat();
            catWinButton.SetActive(true);
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

    public List<PlayerInfo> GetCurrentPlayers()
    {
        var players = new List<PlayerInfo>();
        foreach (var photonPlayer in PhotonNetwork.PlayerList)
        {
            // Build your PlayerInfo from CustomProperties or another source
            PlayerInfo info = new PlayerInfo();
            info.playerId = photonPlayer.ActorNumber;
            info.playerName = photonPlayer.NickName;
            GameObject playerObj;
            if (playerObjects.TryGetValue(photonPlayer.ActorNumber, out playerObj))
            {
                SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
                info.dogColor = sr.color;
                info.isDead = playerObj.GetComponent<PlayerController>().isDead;
            }
            players.Add(info);
        }
        return players;
    }

    [PunRPC]
    void RPC_Meeting()
    {
        var players = InGameController.Instance.GetCurrentPlayers();
        var allControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        var myPlayerController = allControllers.FirstOrDefault(p => p.myPov.IsMine);
        myPlayerController.myPov.RPC("RPC_ResetVote", RpcTarget.All);
        myPlayerController.DisableInputActions();
        meetingPanel.Show(players, (id) => myPlayerController.OnPlayerVote(id));
    }

    // Called via RPC from clients when they vote
    [PunRPC]
    public void RPC_SubmitVote(int votedPlayerId, int voterActorNumber)
    {
        // Only handle on MasterClient
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Add or update vote
        votes[voterActorNumber] = votedPlayerId;

        // Optional: notify UI of vote status here

        // Check if all alive players have voted
        if (votes.Count >= GetAlivePlayerCount())
        {
            TallyVotesAndExecute();
            ClearVotes();
        }
    }

    private int GetAlivePlayerCount()
    {
        // You should customize this to only count players who are alive and can vote.
        // For simplicity, counting all players in the room:
        return PhotonNetwork.PlayerList.Length;
    }

    private void TallyVotesAndExecute()
    {
        // Tally the votes: count occurrences for each votedPlayerId
        Dictionary<int, int> tally = new Dictionary<int, int>();

        foreach (var vote in votes.Values)
        {
            if (tally.ContainsKey(vote))
                tally[vote]++;
            else
                tally[vote] = 1;
        }

        // Find the playerId with the highest votes
        int maxVotes = 0;
        int executedPlayerId = -1;
        bool tie = false;

        foreach (var entry in tally)
        {
            if (entry.Value > maxVotes)
            {
                maxVotes = entry.Value;
                executedPlayerId = entry.Key;
                tie = false;
            }
            else if (entry.Value == maxVotes)
            {
                tie = true;
            }
        }

        if (!tie && executedPlayerId != -1)
        {
            // Execute the player with the most votes
            myPov.RPC("RPC_ExecutePlayer", RpcTarget.All, executedPlayerId);
        }
        else
        {
            // Handle tie: no one is executed
            myPov.RPC("RPC_NoExecution", RpcTarget.All);
        }
    }

    private void ClearVotes()
    {
        votes.Clear();
    }

    [PunRPC]
    public void RPC_ExecutePlayer(int executedPlayerId)
    {
        // Handle execution (kick, mark as dead, etc.)
        Debug.Log($"Player {executedPlayerId} is executed!");

        // Example: Find the GameObject and mark as dead
        if (playerObjects.TryGetValue(executedPlayerId, out GameObject playerObj))
        {
            var controller = playerObj.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.myPov.RPC("RPC_DieByVote", RpcTarget.All);
            }
        }

        // Hide meeting panel, resume gameplay,
        myPov.RPC("RPC_CloseMeetingPanel", RpcTarget.All);
        var allControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        var myPlayerController = allControllers.FirstOrDefault(p => p.myPov.IsMine);
        myPlayerController.EnableInputActions();
    }

    [PunRPC]
    public void RPC_NoExecution()
    {
        Debug.Log("No one executed due to tie!");

        // Hide meeting panel, resume gameplay, etc.
        myPov.RPC("RPC_CloseMeetingPanel", RpcTarget.All);
        var allControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        var myPlayerController = allControllers.FirstOrDefault(p => p.myPov.IsMine);
        myPlayerController.EnableInputActions();
    }

    public void OnClickCatWin()
    {
        PhotonNetwork.LoadLevel("5_EndGame");
    }

    [PunRPC]
    void RPC_CloseMeetingPanel()
    {
        meetingPanel.Hide();
    }
}
