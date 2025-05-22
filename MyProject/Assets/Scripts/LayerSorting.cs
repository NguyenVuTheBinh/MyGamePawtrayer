using UnityEngine;

public class LayerSorter : MonoBehaviour
{
    private SpriteRenderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Wall")) return;

        float playerTopY = GetTopY();
        float wallTopY = other.bounds.max.y;

        int order = playerTopY > wallTopY ? 5 : 7;

        foreach (var r in renderers)
            r.sortingOrder = order;
    }

    float GetTopY()
    {
        Bounds combinedBounds = renderers[0].bounds;
        foreach (var r in renderers)
            combinedBounds.Encapsulate(r.bounds);
        return combinedBounds.max.y;
    }
}
