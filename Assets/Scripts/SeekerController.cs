using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PurrNet;

public class SeekerController : NetworkBehaviour
{
    public NavMeshAgent seeker;
    public SeekerVision seekerVision;

    public LayerMask whatIsGround, whatIsPlayer;

    public Transform[] Targets;

    
    void Start()
    {
        //player = GameObject.Find("Player").transform;
        seeker = GetComponent<NavMeshAgent>();
    }

    void Awake()
    {
        // Grab components if not wired in the inspector
        if (!seeker) seeker = GetComponent<NavMeshAgent>();
        if (!seekerVision) seekerVision = GetComponent<SeekerVision>();
    }

    void Update()
    {
        // Only the server/host should control AI movement
        if (!isServer) return;
        if (seekerVision == null || seeker == null)
            return;
        Transform seenPlayer = seekerVision.playerSeen();

        if (seenPlayer != null)
        {
            SeesPlayer(seenPlayer);
        }
    }

    void SeesPlayer(Transform newTarget)
    {
        if (newTarget == null || seeker == null)
            return;
        if (seeker.hasPath)
        {
            CompareTarget(newTarget);
        }
        else seeker.SetDestination(newTarget.position);
    }
    public void HeardNoise(Transform newTarget)
    {   //If the seeker does not see a player, go to the noise
        if (!isServer) return;  // safety
        if (!seekerVision.canSeePlayer())
        {
            //ChasePlayer();
            //If the seeker has a target, compare the remaining distance from the curTarget to the distance from the newTarget
            if (seeker.hasPath)
            {
                CompareTarget(newTarget);
                //Debug.Log("TARGET UPDATED");
            }
            else 
            {
                seeker.SetDestination(newTarget.position);
                //Debug.Log("TARGET SET");
            }
        }
        else return;
    }

    public void CompareTarget(Transform newTarget)
    {
        NavMeshPath newPath = null;

            if(newTarget == null)
            {
                return;
            }
            newPath = new NavMeshPath();

            if(NavMesh.CalculatePath(transform.position, newTarget.position, seeker.areaMask, newPath))
            {
                float distance = Vector3.Distance(transform.position, newPath.corners[0]);

                for(int i = 1; i < newPath.corners.Length; i++)
                {
                    distance += Vector3.Distance(newPath.corners[i-1], newPath.corners[i]);
                }

                if(distance < seeker.remainingDistance)
                {
                    //seeker.SetPath(newPath);
                    seeker.SetDestination(newTarget.position);
                    return;
                }
                else return;
            }
            else return;
    }
}
