using UnityEngine;
using UnityEngine.UI;

public class Interactive : MonoBehaviour
{
    [SerializeField] GameObject miniGame;
    GameObject highlight;
    [SerializeField] GameObject Interact;
    private void OnEnable()
    {
        highlight = transform.GetChild(0).gameObject;
    }
    public void CloseToInteract()
    {
        highlight.SetActive(true);
        Interact.SetActive(true);
    }
    public void FarToInteract()
    {
        highlight.SetActive(false);
        Interact.SetActive(false);
    }
    public void PlayMiniGame()
    {
        miniGame.SetActive(true);
    }


}
