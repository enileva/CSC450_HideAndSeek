using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerNoiseDetector : MonoBehaviour
{
    public Transform seeker;
    public Transform player;
    public SeekerController seekerController;
    GameObject attachedGameObject;
    /*
    public string tagFilter;
    public UnityEvent OnTriggerEnter;
    void OnTriggerEnter(Collider seeker)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !seeker.gameObject.CompareTag(tagFilter)) return;
        OnTriggerEnter.Invoke();
    }
    */

    void Start()
    {
        seeker = GameObject.Find("Seeker").transform;
        player = GameObject.Find("Player").transform;
        attachedGameObject = gameObject;
    }

    void OnTriggerStay(Collider thing)
    { // Check if seeker enters noise sphere
        if (thing.CompareTag("Seeker"))
        {
            // Inform seeker of player location
            //Debug.Log(thing.gameObject + " hears " + attachedGameObject);
            //seekerController.HeardNoise(player);
        }
    }
    void OnTriggerEnter(Collider thing)
    {
        if (thing.CompareTag("Seeker"))
        {
            // Inform seeker of player location
            Debug.Log(thing.gameObject + " hears " + attachedGameObject);
            //seekerController.HeardNoise(thing.transform);
        }
    }
}
