using UnityEngine;

public class NPCFollowLook : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool onlyRotateY = true;

    [Header("Orientation Fix")]
    [Tooltip("Regola questo valore se l'NPC ti guarda di fianco (es. 90, -90 o 180)")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0, 0);

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;

        if (onlyRotateY) direction.y = 0;

        if (direction != Vector3.zero)
        {
            // Calcolo la rotazione base verso il player
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Applico l'offset locale per correggere il modello
            Quaternion finalRotation = lookRotation * Quaternion.Euler(rotationOffset);

            // Applico la rotazione fluida
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
        }
    }
}