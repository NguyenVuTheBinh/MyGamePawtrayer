using Photon.Pun;
using UnityEngine;

public class ColorSetUp : MonoBehaviour
{
    public GameObject getColor;
    void Start()
    {
        if (getColor.GetComponent<PlayerSetColor>().allColors == null)
        {
            this.enabled = false;
            return;
        }
        else
        {
            this.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Set and sync color
    public void SetColor(Color newColor)
    {
        if (!myPov)
            return;
        if (dogSprite != null)
        {
            myPov.RPC("RPC_SetColor", RpcTarget.All, newColor);
        }
    }
    [PunRPC]
    void RPC_SetColor(Color newColor)
    {
        dogColor = newColor;
        dogSprite.color = dogColor;
    }
}
