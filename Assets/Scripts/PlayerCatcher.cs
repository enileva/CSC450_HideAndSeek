using UnityEngine;

public class PlayerCatcher : MonoBehaviour
{
    public float damage = 1f;
    public LayerMask obstacleMask = ~0;   // layers that can block damage (default: everything)

    private Transform seekerRoot;
    private Collider seekerCollider;

    void Awake()
    {
        seekerRoot = transform.root;
        seekerCollider = seekerRoot.GetComponent<Collider>();
        if (!seekerCollider)
            Debug.LogWarning("PlayerCatcher: No collider found on seeker root. Line-of-sight may be off.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Get positions to raycast between
        Vector3 from = seekerCollider ? seekerCollider.bounds.center : seekerRoot.position;
        Vector3 to = other.bounds.center;

        // Check if something is between seeker and player
        if (Physics.Linecast(from, to, out RaycastHit hit, obstacleMask))
        {
            // If we hit something that is NOT the player (or a child of the player),
            // then a wall or obstacle is in the way → no damage.
            if (hit.collider != other &&
                !hit.collider.transform.IsChildOf(other.transform))
            {
                // Blocked by wall, bail out
                // Debug.Log("Damage blocked by " + hit.collider.name);
                return;
            }
        }

        // Actually apply damage
        var receiver = other.GetComponentInParent<PlayerDamageReceiver>();
        if (receiver != null)
        {
            receiver.TakeDamage(damage);
            Debug.Log("YOU'VE BEEN CAUGHT! " + other.name);
        }
        else
        {
            Debug.LogWarning("PlayerCatcher: Player hit but no PlayerDamageReceiver on " + other.name);
        }
    }
}
