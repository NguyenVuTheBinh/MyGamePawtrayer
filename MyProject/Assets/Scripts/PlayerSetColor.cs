using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetColor : MonoBehaviour
{
    [SerializeField] Color[] allColors;

    public void SetColor(int colorIndex)
    {
        PlayerController.localPlayer.SetColor(allColors[colorIndex]);
    }
    public void NextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
