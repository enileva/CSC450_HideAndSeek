using UnityEngine;
using PurrNet;

public class PlayerDamageReceiver : NetworkBehaviour
{
    private GameUI ui;
    // prevent rapid-fire damage
    public float damageCooldown = 1f;
    private float lastDamageTime = -999f;

    void Start()
    {
        // Only the local owner cares about its own UI + health
        if (isOwner)
        {
            ui = FindObjectOfType<GameUI>();
        }
    }

    public void TakeDamage(float amount)
    {
        // Only process damage + UI for the local player
        if (!isOwner) return;
        // cooldown
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        if (ui != null)
        {
            ui.TakeDamage(amount);
        }
        else
        {
            Debug.LogWarning("PlayerDamageReceiver: GameUI not found.");
        }
    }
}
