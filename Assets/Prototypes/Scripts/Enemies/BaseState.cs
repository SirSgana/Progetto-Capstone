using UnityEngine;

public abstract class BaseState
{
    protected EnemyController enemy;
    public BaseState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public abstract void StateEnter();      //Esegui all'inizio dello stato
    public abstract void StateUpdate();     // Esegui ad ogni frame
    public abstract void StateExit();       // Esegui all'uscita dello stato
    public abstract void CheckTransition(); // Controlla condizione di cambio stato

}
