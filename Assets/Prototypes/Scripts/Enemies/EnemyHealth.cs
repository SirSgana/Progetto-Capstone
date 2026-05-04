using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public void Damage(int damage)
    {
        Die();
    }

    private void Die()
    {
        Debug.Log("Nemico eliminato!");
     
        if (EnemyDefeatCount.Instance != null)
        {
            EnemyDefeatCount.Instance.AddEnemyKilled();
        }

        Destroy(gameObject);
    }
}
