using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseState
{
    public IdleState(EnemyController enemy) : base(enemy) { }

    private float idleTimer;


    public override void StateEnter()
    {
        enemy.agent.isStopped = true;
        enemy.anim.SetBool("IsPatrolling", false);
        idleTimer = 0f;
    }

    public override void StateUpdate() 
    { 
        idleTimer += Time.deltaTime;
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
        if (idleTimer >= enemy.idleDuration)
        {
            enemy.ChangeState(enemy.patrolState);
        }
    }
}
