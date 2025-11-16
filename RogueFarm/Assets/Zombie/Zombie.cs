using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private NavMeshAgent Agent;
    private Player Player;
    private Vector3 SpawnPosition;
    
    public void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = FindAnyObjectByType<Player>();
        SpawnPosition = transform.position;
    }

    public void Update()
    {
        Agent.destination = Player.transform.position;
    }

    public void Hit()
    {
        Agent.Warp(SpawnPosition);
    }

}
