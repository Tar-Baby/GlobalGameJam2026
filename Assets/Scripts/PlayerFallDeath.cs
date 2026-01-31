using UnityEngine;

public class PlayerFallDeath : MonoBehaviour
{
    [SerializeField] private float deathY = -10f;
    private bool isDead = false;

    void Update()
    {
        if (!isDead && transform.position.y < deathY)
        {
            isDead = true;
            GameManager.Instance.GameOver();
        }
    }
}
