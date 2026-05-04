using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState
{
    private Vector3 targetPoint;

    public PatrolState(EnemyController enemy) : base(enemy) { }

    public override void StateEnter()
    {
        enemy.agent.isStopped = false;
        enemy.anim.SetBool("IsPatrolling", true);
        SetNewRandomDestination();
    }

    public override void StateUpdate()
    {
        // Se il nemico è quasi arrivato al punto, ne sceglie un altro
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.5f)
        {
            SetNewRandomDestination();
            enemy.ChangeState(enemy.idleState);
        }
    }

    public override void StateExit() { }

    public override void CheckTransition()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        //Controllo distanza
        if (distance < enemy.distanceToPlayer)
        {
            //Controllo direzione verso il player
            Vector3 directionToPlayer = (enemy.player.position - enemy.transform.position).normalized;

            //Calcolo angolo tra il davanti dell'enemy ed il player
            float angle = Vector3.Angle(enemy.transform.forward, directionToPlayer);

            //Se l'angolo è stretto allora lo vede
            if (angle < enemy.angleOfView)
            {
                enemy.ChangeState(enemy.chaseState);
                return;
            }
        }
    }

    private void SetNewRandomDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * enemy.patrolRadius;
        randomDir += enemy.transform.position;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDir, out hit, enemy.patrolRadius, 1);
        enemy.agent.SetDestination(hit.position);
    }
}