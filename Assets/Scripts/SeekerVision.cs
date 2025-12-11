using UnityEngine;

public class SeekerVision : MonoBehaviour
{

    bool inRange, inAngle, isNotHidden;
    public bool seesPlayer;
    public float visionRange = 10;
    public float visionAngle = 45;
    public GameObject Player;

    void Start()
    {
        seesPlayer = false;
    }

    void Update()
    {
        #region updateBools

            if (Vector3.Distance(transform.position, Player.transform.position) < visionRange)
                inRange = true;
            else inRange = false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, (Player.transform.position - transform.position), out hit, Mathf.Infinity))
            {
                if (hit.transform == Player.transform)
                    isNotHidden = true;
                else isNotHidden = false;
            }
            else isNotHidden = false;

            Vector3 side1 = Player.transform.position - transform.position;
            Vector3 side2 = transform.forward;
            float angle = Vector3.SignedAngle(side1, side2, Vector3.up);
            if (angle < visionAngle && angle > -1 * visionAngle)
                inAngle = true;
            else inAngle = false;
            
        #endregion

        seesPlayer = canSeePlayer();

    }
    
    public bool canSeePlayer()
    {
        return (inRange && inAngle && isNotHidden);
    }
    public Transform playerSeen()
    {
        if (canSeePlayer())
        {
            return Player.transform;
        }
        else
        {
            return null;
        }
    }

}
