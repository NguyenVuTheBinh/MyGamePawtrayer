using Unity.VisualScripting;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    public void SetColor(Color dogColor)
    {
        spriteRenderer.color = dogColor;
    }
    private void OnEnable()
    {
        if (PlayerController.allBodies != null)
        {
            PlayerController.allBodies.Add(transform);
        }
    }
    public void Report()
    {
        Debug.Log("report");        
        foreach (var body in PlayerController.allBodies)        
            Destroy(body.gameObject);
        PlayerController.allBodies.Clear();
    }
}
