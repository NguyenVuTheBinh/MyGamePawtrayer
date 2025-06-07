using UnityEngine;

public class Follow_player : MonoBehaviour
{

    public Transform player;

    Vector3 camOffset;
    void Start()
    {
        camOffset = transform.position - player.position;
    }

    private void FixedUpdate()
    {
        transform.position = player.position + camOffset;
    }
}
