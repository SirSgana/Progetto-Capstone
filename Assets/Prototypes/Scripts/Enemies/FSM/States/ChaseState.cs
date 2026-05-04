using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BaseState
{
    public ChaseState(EnemyController enemy) : base(enemy) { }

    public override void StateEnter()
    {
        enemy.agent.isStopped = false;

        enemy.anim.SetBool("IsPlayerChaseRange", true);
        enemy.anim.SetBool("IsPatrolling", false);

        Debug.Log("Inizio ChaseState");
    }

    public override void StateUpdate() 
    { 
        if (enemy.player != null)
        {
            enemy.agent.SetDestination(enemy.player.position);
        }
    }
    public override void StateExit() 
    { 
        enemy.anim.SetBool("IsPlayerChaseRange", false);
    }
    public override void CheckTransition() 
    { 
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distance < enemy.attackRange)
        {
            enemy.ChangeState(enemy.attackState);
        }
        else if (distance > enemy.distanceToPlayer)
        {
            enemy.ChangeState(enemy.idleState);
        }
    }
}
