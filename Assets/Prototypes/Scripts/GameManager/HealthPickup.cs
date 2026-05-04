using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _healAmount = 20;

    [Header("Rotation Settings")]
    [SerializeField] private float _xAngle = 0f;
    [SerializeField] private float _yAngle = 100f; 
    [SerializeField] private float _zAngle = 0f;

    private void Update()
    {
        
        transform.Rotate(
            _xAngle * Time.deltaTime,
            _yAngle * Time.deltaTime,
            _zAngle * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LifeController life = other.GetComponent<LifeController>();

            if (life != null)
            {
                if (life.GetHp() >= 100)
                {
                    Debug.Log("Vita già al massimo!");
                    return;
                }

                life.Heal(_healAmount);
                Destroy(gameObject);
            }
        }
    }
}