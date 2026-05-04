using UnityEngine;
using UnityEngine.UI;

public class LifeController : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private int _minHp = 0;
    [SerializeField] private Slider _healthSlider;

    private int _currentHp;

    private void Awake()
    {
        _currentHp = _maxHp;
        _healthSlider.maxValue = _maxHp;
        _healthSlider.value = _currentHp;
    }
    public int GetHp() => _currentHp;

    private void SetHp(int hp)
    {
        _currentHp = Mathf.Clamp(hp, _minHp, _maxHp);
        _healthSlider.value = _currentHp;
    }

    public void Damage(int damage)
    {
        SetHp(_currentHp - damage);
        //CheckHp();
        Debug.Log("Il player ha ancora: " + _currentHp + " Vita rimasta");
    }

    public void ResetHp()
    {
        _currentHp = _maxHp;
    }

    public void Heal(int amount)
    {
        SetHp(_currentHp + amount);
        Debug.Log("Vita recuperata! Attuale: " + _currentHp);
    }
}