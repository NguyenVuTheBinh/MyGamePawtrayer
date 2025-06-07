using UnityEngine;

public class MiniGameNumberOrder : MonoBehaviour
{
    [SerializeField] int nextButton;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject[] myObjects;
    void Start()
    {
        nextButton = 0;
    }
    private void OnEnable()
    {
        nextButton = 0;
        for (int i = 0; i < myObjects.Length; i++)
        {
            myObjects[i].transform.SetSiblingIndex(Random.Range(0, 9));
        }
    }
     public void ButtonOrder(int button)
    {
        Debug.Log("pressed");
        if (button == nextButton)
        {
            nextButton++;
            Debug.Log("next button" + nextButton);
        } else
        {
            Debug.Log("fail");
            Debug.Log("nexr button" + nextButton);
            nextButton = 0;
            OnEnable();
        }
        if (button == 9 && button == nextButton-1)
        {
            Debug.Log("pass");
            nextButton = 0;
            ButtonOrderPanelClose();
        }
    }
    public void ButtonOrderPanelClose()
    {
        gamePanel.SetActive(false);
    }
    public void ButtonOrderPanelOpen()
    {
        gamePanel.SetActive(true);
    }
}
