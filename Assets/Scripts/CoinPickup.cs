using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to Player-tagged objects
        if (!other.CompareTag("Player")) return;

        // Find the PlayerCoinCollector on the player (or its parent)
        var collector = other.GetComponentInParent<PlayerCoinCollector>();
        if (collector != null)
        {
            collector.CollectCoins(value);
        }
        else
        {
            Debug.LogWarning("CoinPickup: Player hit but no PlayerCoinCollector found on " + other.name);
        }

        // Destroy this coin on this client
        Destroy(gameObject);
    }
}
