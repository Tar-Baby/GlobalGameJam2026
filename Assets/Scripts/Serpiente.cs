using System.Collections.Generic;
using UnityEngine;

public class Serpiente : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string tubeTag = "Tubos";

    private Collider2D playerCollider;
    private bool canPhase;

    private readonly List<Collider2D> ignoredColliders = new();

    void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    public void EnablePhase()
    {
        canPhase = true;
    }

    public void DisablePhase()
    {
        canPhase = false;
        RestoreAllCollisions();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canPhase)
        {
            return;
        }

        if (!collision.collider.CompareTag(tubeTag))
        {
            return;
        }

        Physics2D.IgnoreCollision(playerCollider, collision.collider, true);
        ignoredColliders.Add(collision.collider);
    }

    void RestoreAllCollisions()
    {
        foreach (Collider2D col in ignoredColliders)
        {
            if (col != null)
            {
                Physics2D.IgnoreCollision(playerCollider, col, false);
            }
        }

        ignoredColliders.Clear();
    }
}