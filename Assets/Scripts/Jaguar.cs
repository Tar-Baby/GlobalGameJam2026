using UnityEngine;

public class Jaguar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float destroyRange = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    public void TryDestroyObstacle()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            destroyRange,
            obstacleLayer
        );

        if (hit == null)
        {
            return;
        }

        if (!hit.CompareTag("Obstaculo"))
        {
            return;
        }

        Destroy(hit.gameObject);
        Debug.Log("Obstáculo destruido: " + hit.name);
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destroyRange);
    }
}