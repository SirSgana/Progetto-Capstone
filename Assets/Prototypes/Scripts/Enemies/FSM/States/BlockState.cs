using UnityEngine;
using UnityEngine.AI;

public class BlockState : BaseState
{
    public BlockState(EnemyController enemy) : base(enemy) { }

    public override void StateEnter()
    {
        enemy.anim.SetBool("CanBlock", true);
    }

    public override void StateUpdate() { }
    public override void StateExit() { }
    public override void CheckTransition() { }
}