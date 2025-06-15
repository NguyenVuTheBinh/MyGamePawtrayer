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
    public void UpdateCullingMask(bool isDead)
    {
        if (isDead)
            Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("DeadPlayerLayer");
        else
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("DeadPlayerLayer"));
    }
}
