using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerSetColor : MonoBehaviour
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

    public Color[] allColors;
    [SerializeField] GameObject colorPanel;
    private void Awake()
    {
        allColors = new Color[] {color1, color2, color3, color4, color5, color6, color7, color8, color9};
    }
    public void ChangeColor(int colorIndex)
    {
        PlayerController.localPlayer.SetColor(allColors[colorIndex]);
    }
    public void ButtonOrderPanelClose()
    {
        colorPanel.SetActive(false);
    }
}
