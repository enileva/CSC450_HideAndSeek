using UnityEngine;
using PurrNet;

public class PlayerCoinCollector : NetworkBehaviour
{
    private GameUI ui;
    private AudioSource coinAudio;

    void Start()
    {
        // Only the local owner cares about its own UI
        if (isOwner)
        {
            ui = FindObjectOfType<GameUI>();
            coinAudio = GetComponent<AudioSource>();
        }
    }

    public void CollectCoins(int amount)
    {
        // Only update UI for the local player
        if (!isOwner) return;

        if (ui != null)
        {
            ui.AddCoins(amount);
        }
        else
        {
            Debug.LogWarning("PlayerCoinCollector: GameUI not found.");
        }
        if (coinAudio != null)
            coinAudio.Play();
        else
            Debug.LogWarning("No AudioSource found for coin pickup sound!");
    }
}
