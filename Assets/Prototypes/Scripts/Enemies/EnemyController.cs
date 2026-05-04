using UnityEngine.AI;
using EnemyBaseState.FSM;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;

    [Header("Settings")]
    public float patrolRadius = 3f;
    public float distanceToPlayer = 7f;
    public float attackRange = 1.5f;
    public float angleOfView = 75f;
    public float idleDuration = 3f;

    private BaseState currentState;

    public IdleState idleState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public AttackState attackState;
    public BlockState blockState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //Inizializzo gli stati
        idleState = new IdleState(this);
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        blockState = new BlockState(this);

        //Imposto lo stato iniziale
        currentState = idleState;
    }

    private void Update()
    {
        currentState?.StateUpdate();

        currentState?.CheckTransition();
    }

    public void ChangeState(BaseState newState)
    {
        currentState?.StateExit();
        currentState = newState;
        currentState?.StateEnter();

        Debug.Log("Nuovo Stato: " + newState.GetType().Name);
    }

    private void OnDrawGizmos()
    {
        // Disegna il raggio di rilevamento
        if (transform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceToPlayer);

        Gizmos.color = Color.red;
        Vector3 leftBourdary = Quaternion.Euler(0, -angleOfView, 0) * transform.forward;
        Vector3 rightBourdary = Quaternion.Euler(0, angleOfView, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftBourdary * distanceToPlayer);
        Gizmos.DrawRay(transform.position, rightBourdary * distanceToPlayer);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
