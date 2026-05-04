using UnityEngine;
using TMPro;

public class EnemyDefeatCount : MonoBehaviour
{
    public static EnemyDefeatCount Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int _targetKills = 21;
    [SerializeField] private TextMeshProUGUI _counterText;

    private int _enemiesDefeated = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddEnemyKilled()
    {
        _enemiesDefeated++;
        UpdateUI();

        if (_enemiesDefeated >= _targetKills)
        {
            Debug.Log("Obiettivo raggiunto!");
        }
    }
    
    private void UpdateUI()
    {
        if (_counterText != null)
        {
            _counterText.text = $"Enemies Defeated: {_enemiesDefeated} / {_targetKills}";
        }
    }

    public int GetTotalKills()
    {
        return _enemiesDefeated;
    }
}