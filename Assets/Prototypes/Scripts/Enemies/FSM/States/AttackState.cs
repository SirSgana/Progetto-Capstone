using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseState
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    public AttackState(EnemyController enemy) : base(enemy) { }

    public override void StateEnter()
    {
        enemy.agent.isStopped = true;

        enemy.anim.SetBool("IsPlayerAttackRange", true);

        Attack();
    }

    public override void StateUpdate() 
    {
        LookAtPlayer();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }
    public override void StateExit() 
    { 
        enemy.anim.SetBool("IsPlayerAttackRange", false);
        enemy.agent.isStopped = false;
    }

    public override void CheckTransition() 
    { 
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distance > enemy.attackRange + 0.5f)
        {
            enemy.ChangeState(enemy.chaseState);
        }
    }

    private void Attack()
    {
        Debug.Log("Attacco!");
        lastAttackTime = Time.time;
        enemy.anim.SetTrigger("Attack");
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        direction.y = 0; // Mantiene l'orientamento solo sull'asse Y
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

}
