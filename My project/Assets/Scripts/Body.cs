using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Body : MonoBehaviourPun
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public PhotonView myPov;
    private void Start()
    {
        myPov = gameObject.GetComponent<PhotonView>();
    }
    [PunRPC]
    public void RPC_SetColorDeadBody(float r, float g, float b, float a)
    {
        spriteRenderer.color = new Color(r, g, b, 1f);
        Debug.Log($"Set dead body color to: {r}, {g}, {b}, {a}");
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
