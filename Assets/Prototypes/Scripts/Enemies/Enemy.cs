using EnemyBaseState.FSM;
using EnemyStateEvent.FSM;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(requiredComponent:typeof(Animator), requiredComponent2:typeof(NavMeshAgent))]

public class Enemy : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
}
