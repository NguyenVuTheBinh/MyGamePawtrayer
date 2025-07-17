using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class StartGameController: MonoBehaviourPunCallbacks
{
    public GameObject ConnectServerCanvas;
    public GameObject GameMenuCanvas;
    public GameObject holdServer;
    public Button pickHongkong;
    public Button pickSingapore;
    public InputField usernameInput;
    public Text buttonText;

    private void Start()
    {
        ShowGameMenu();
    }
    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Just a second :D";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("2_Lobby");
    }

    public void OnClickStartGame()
    {
        ShowConnect();
    }
    public void OnClickBackMenu()
    {
        ShowGameMenu();
    }
    public void OnClickQuitGame()
    {
        Application.Quit();
    }
    public void OnClickShowServerOptions()
    {
        holdServer.SetActive(!holdServer.activeSelf);
    }
    public void OnClickPickHongkong()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "hk";
    }
    public void OnClickPickSingapore()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
    }
    void ShowConnect()
    {
        ConnectServerCanvas.SetActive(true);
        GameMenuCanvas.SetActive(false);
    }
    void ShowGameMenu()
    {
        ConnectServerCanvas.SetActive(false);
        GameMenuCanvas.SetActive(true);
    }
}
