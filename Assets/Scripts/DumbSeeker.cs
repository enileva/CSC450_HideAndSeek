using UnityEngine;
using PurrNet;   // important

public class DumbSeeker : NetworkBehaviour
{
    public string playerTag = "Player";
    public float speed = 3f;
    public float detectionRange = 50f;

    private Transform target;

    void Update()
    {
        // Only the server decides AI movement
        if (!isServer) return;
        if (target == null || !target.CompareTag(playerTag))
        {
            UpdateTarget();
        }

        UpdateTarget();

        if (target == null) return;

        // Move toward the target in a straight line on the XZ plane
        Vector3 to = target.position - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude < 0.01f) return;

        to.Normalize();

        transform.position += to * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(to, Vector3.up);
    }

    void UpdateTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        if (players.Length == 0)
        {
            target = null;
            return;
        }

        Transform best = null;
        float bestDist = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (var p in players)
        {
            // Just in case the seeker somehow has the same tag, never pick ourselves
            if (p.transform == transform) continue;

            float d = Vector3.Distance(myPos, p.transform.position);
            if (d < bestDist && d <= detectionRange)
            {
                bestDist = d;
                best = p.transform;
            }
        }

        target = best;
    }
}
