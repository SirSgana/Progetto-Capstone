using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int _attackDamage = 1;
    [SerializeField] private Collider _weaponCollider; 
    [SerializeField] private float _attackDuration = 0.5f; 

    private void Start()
    {
        // All'inizio l'arma non deve fare danno
        if (_weaponCollider != null)
            _weaponCollider.enabled = false;
    }

    private void Update()
    {
        // Se premo il tasto sinistro del mouse, attivo l'attacco
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        _weaponCollider.enabled = true; // Attiva il collider
        Debug.Log("Attacco iniziato!");

        yield return new WaitForSeconds(_attackDuration);

        _weaponCollider.enabled = false; // Disattiva il collider
        Debug.Log("Attacco terminato!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(_attackDamage);
            }
        }
    }
}